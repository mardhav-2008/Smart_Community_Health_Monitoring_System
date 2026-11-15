
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Areas.Admin.ViewModels;
using Smart_Community_Health_Monitoring_System.Models.Identity;

namespace Smart_Community_Health_Monitoring_System.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdministratorOnly")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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

        if (model.Role != "Administrator" && model.Role != "FieldWorker")
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

        var currentRoles = await _userManager.GetRolesAsync(user);
        var currentlyAdmin = currentRoles.Contains("Administrator");
        var willBeAdmin = model.Role == "Administrator";

        // Prevent administrators from locking themselves out.
        if (isCurrentUser && (!model.IsActive || !willBeAdmin))
        {
            ModelState.AddModelError(
                string.Empty,
                "You cannot deactivate your own account or remove your Administrator role.");

            return View(model);
        }

        // Protect the last active administrator.
        if (currentlyAdmin && (!willBeAdmin || !model.IsActive))
        {
            var admins = await _userManager.GetUsersInRoleAsync(
                "Administrator");

            var otherActiveAdmins = admins.Count(admin =>
                admin.Id != user.Id && admin.IsActive);

            if (!model.IsActive || otherActiveAdmins == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The last active administrator cannot be demoted or deactivated.");

                return View(model);
            }
        }

        // Prevent duplicate email addresses.
        var email = model.Email.Trim();
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null && existingUser.Id != user.Id)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "This email address is already in use.");

            return View(model);
        }

        var wasActive = user.IsActive;
        var roleChanged = currentlyAdmin != willBeAdmin;

        var securitySensitiveChange =
            roleChanged || wasActive != model.IsActive;

        // Apply role changes and check the result.
        if (roleChanged)
        {
            var roleResult = willBeAdmin
                ? await _userManager.AddToRoleAsync(user, "Administrator")
                : await _userManager.RemoveFromRoleAsync(
                    user,
                    "Administrator");

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);

                return View(model);
            }
        }

        // Update account properties.
        user.DisplayName = model.DisplayName.Trim();
        user.Email = email;
        user.UserName = email;
        user.IsActive = model.IsActive;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            // Roll back the role change if the account update fails.
            if (roleChanged)
            {
                var rollbackResult = currentlyAdmin
                    ? await _userManager.AddToRoleAsync(
                        user,
                        "Administrator")
                    : await _userManager.RemoveFromRoleAsync(
                        user,
                        "Administrator");

                if (!rollbackResult.Succeeded)
                {
                    foreach (var error in rollbackResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            $"Role rollback failed: {error.Description}");
                    }
                }
            }

            foreach (var error in updateResult.Errors)
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);

            return View(model);
        }

        // Invalidate existing sessions after role/status changes.
        if (securitySensitiveChange)
        {
            var stampResult =
                await _userManager.UpdateSecurityStampAsync(user);

            if (!stampResult.Succeeded)
            {
                foreach (var error in stampResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }
        }

        TempData["SuccessMessage"] =
            $"Updated account for {user.DisplayName}.";

        return RedirectToAction(nameof(Index));
    }
}
