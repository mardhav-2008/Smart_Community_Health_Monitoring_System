
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Areas.Admin.ViewModels;
using Smart_Community_Health_Monitoring_System.Models.Identity;
using Smart_Community_Health_Monitoring_System.Data;

namespace Smart_Community_Health_Monitoring_System.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdministratorOnly")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // GET: /Admin/Users
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderBy(user => user.DisplayName)
            .ToListAsync();

        var userList = new List<UserListViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            userList.Add(new UserListViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName ?? "(No display name)",
                Email = user.Email ?? "(No email)",
                Role = roles.FirstOrDefault() ?? "No role",
                IsActive = user.IsActive,
                CreatedAtUtc = user.CreatedAtUtc ?? DateTime.MinValue
            });
        }

        return View(userList);
    }

    // GET: /Admin/Users/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateUserViewModel());
    }

    // POST: /Admin/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Role != "Administrator" && model.Role != "FieldWorker")
        {
            ModelState.AddModelError(
                nameof(model.Role),
                "Invalid role selected.");

            return View(model);
        }

        var email = model.Email.Trim();

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = model.DisplayName.Trim(),
            EmailConfirmed = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, model.Role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            foreach (var error in roleResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        TempData["SuccessMessage"] =
            $"Account created for {user.DisplayName}.";

        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Users/Edit/{id}
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return View(new EditUserViewModel
        {
            Id = user.Id,
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Role = roles.FirstOrDefault() ?? "FieldWorker",
            IsActive = user.IsActive
        });
    }

    // POST: /Admin/Users/Edit

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Role != "Administrator" &&
            model.Role != "FieldWorker")
        {
            ModelState.AddModelError(
                nameof(model.Role),
                "Invalid role selected.");

            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.Id);

        if (user == null)
            return NotFound();

        var currentUserId = _userManager.GetUserId(User);
        var isCurrentUser = currentUserId == user.Id;

        var currentRoles = (await _userManager.GetRolesAsync(user)).ToList();
        var currentlyAdmin = currentRoles.Contains("Administrator");
        var willBeAdmin = model.Role == "Administrator";
        var wasActive = user.IsActive;

        // Prevent self-lockout.
        if (isCurrentUser && (!model.IsActive || !willBeAdmin))
        {
            ModelState.AddModelError(
                string.Empty,
                "You cannot deactivate your own account or remove your Administrator role.");

            return View(model);
        }

        // Protect the last active administrator.
        if (currentlyAdmin &&
            user.IsActive &&
            (!willBeAdmin || !model.IsActive))
        {
            var admins = await _userManager.GetUsersInRoleAsync(
                "Administrator");

            var otherActiveAdmins = admins.Count(admin =>
                admin.Id != user.Id && admin.IsActive);

            if (otherActiveAdmins == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The last active administrator cannot be demoted or deactivated.");

                return View(model);
            }
        }

        var email = model.Email.Trim();

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null && existingUser.Id != user.Id)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "This email address is already in use.");

            return View(model);
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // Update email through Identity.
            var emailResult = await _userManager.SetEmailAsync(user, email);

            if (!emailResult.Succeeded)
            {
                foreach (var error in emailResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                await transaction.RollbackAsync();
                return View(model);
            }

            // Keep username synchronized with email.
            var usernameResult = await _userManager.SetUserNameAsync(
                user,
                email);

            if (!usernameResult.Succeeded)
            {
                foreach (var error in usernameResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                await transaction.RollbackAsync();
                return View(model);
            }

            user.DisplayName = model.DisplayName.Trim();
            user.IsActive = model.IsActive;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                await transaction.RollbackAsync();
                return View(model);
            }

            // Ensure exactly one application role is assigned.
            var rolesToRemove = currentRoles
                .Where(role => role != model.Role)
                .ToList();

            foreach (var role in rolesToRemove)
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(
                    user,
                    role);

                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    await transaction.RollbackAsync();
                    return View(model);
                }
            }

            if (!currentRoles.Contains(model.Role))
            {
                var addResult = await _userManager.AddToRoleAsync(
                    user,
                    model.Role);

                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    await transaction.RollbackAsync();
                    return View(model);
                }
            }

            var securitySensitiveChange =
                currentlyAdmin != willBeAdmin ||
                wasActive != model.IsActive;

            // Revoke existing authentication sessions after security changes.
            if (securitySensitiveChange)
            {
                var stampResult =
                    await _userManager.UpdateSecurityStampAsync(user);

                if (!stampResult.Succeeded)
                {
                    foreach (var error in stampResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    await transaction.RollbackAsync();
                    return View(model);
                }
            }

            await transaction.CommitAsync();

            TempData["SuccessMessage"] =
                $"Updated account for {user.DisplayName}.";

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            await transaction.RollbackAsync();

            ModelState.AddModelError(
                string.Empty,
                "An unexpected error occurred while updating the account.");

            return View(model);
        }
    }

}
