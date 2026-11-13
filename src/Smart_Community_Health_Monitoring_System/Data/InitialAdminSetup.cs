
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Smart_Community_Health_Monitoring_System.Models.Identity;

namespace Smart_Community_Health_Monitoring_System.Data;

public static class InitialAdminSetup
{
    public static async Task CreateAdminAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string displayName,
        string password)
    {
        email = email.Trim();
        displayName = displayName.Trim();

        if (!new EmailAddressAttribute().IsValid(email))
        {
            throw new ArgumentException("Invalid email address.");
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.");
        }

        var existingAdmin = await userManager.GetUsersInRoleAsync(
            "Administrator");

        if (existingAdmin.Count > 0)
        {
            throw new InvalidOperationException(
                "An administrator already exists. Setup is disabled.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(
                Environment.NewLine,
                createResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Administrator creation failed:{Environment.NewLine}{errors}");
        }

        var roleResult = await userManager.AddToRoleAsync(
            user, "Administrator");

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);

            var errors = string.Join(
                Environment.NewLine,
                roleResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Administrator role assignment failed:{Environment.NewLine}{errors}");
        }
    }
}
