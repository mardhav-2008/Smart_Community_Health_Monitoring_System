
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Data;
using Smart_Community_Health_Monitoring_System.Models.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var databasePath = Path.GetFullPath(
    Path.Combine(
        builder.Environment.ContentRootPath,
        "../../data/community-health.db"
    )
);

Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}")
);

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 10;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = "CommunityHealth.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Create the required roles.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeeder.SeedRolesAsync(roleManager);
}


if (args.Contains("--setup-admin"))
{
    using var scope = app.Services.CreateScope();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    Console.WriteLine();
    Console.WriteLine("========================================");
    Console.WriteLine("  Community Health System");
    Console.WriteLine("  Initial Administrator Setup");
    Console.WriteLine("========================================");
    Console.WriteLine();

    Console.Write("Administrator email: ");
    var email = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Display name: ");
    var displayName = Console.ReadLine()?.Trim() ?? "";

    if (string.IsNullOrWhiteSpace(email) ||
        !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
            .IsValid(email))
    {
        Console.Error.WriteLine("A valid email address is required.");
        Environment.ExitCode = 1;
        return;
    }

    if (string.IsNullOrWhiteSpace(displayName))
    {
        Console.Error.WriteLine("Display name is required.");
        Environment.ExitCode = 1;
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Password requirements:");
    Console.WriteLine("- At least 10 characters");
    Console.WriteLine("- At least one uppercase letter");
    Console.WriteLine("- At least one lowercase letter");
    Console.WriteLine("- At least one digit");
    Console.WriteLine("- At least one special character");
    Console.WriteLine();

    while (true)
    {
        Console.Write("Administrator password: ");
        var password = ReadPassword();

        Console.Write("Confirm password: ");
        var confirmation = ReadPassword();

        if (password != confirmation)
        {
            Console.WriteLine("Passwords do not match. Try again.");
            Console.WriteLine();
            continue;
        }

        try
        {
            await InitialAdminSetup.CreateAdminAsync(
                userManager,
                email,
                displayName,
                password
            );

            Console.WriteLine();
            Console.WriteLine(
                "Administrator account created successfully.");

            break;
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine();
            Console.WriteLine(exception.Message);
            Console.WriteLine();

            if (exception.Message.Contains(
                "administrator already exists",
                StringComparison.OrdinalIgnoreCase))
            {
                Environment.ExitCode = 1;
                break;
            }

            Console.WriteLine("Please choose another password.");
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
            Environment.ExitCode = 1;
            break;
        }
    }

    return;
}


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await DevelopmentDataSeeder.SeedAsync(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

static string ReadPassword()
{
    var password = new System.Text.StringBuilder();

    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            break;
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (password.Length > 0)
            {
                password.Length--;
            }

            continue;
        }

        if (!char.IsControl(key.KeyChar))
        {
            password.Append(key.KeyChar);
        }
    }

    return password.ToString();
}
