using Microsoft.AspNetCore.Identity;
using SkyrimWIKI.Models;

namespace SkyrimWIKI.Data;

// Creates the "Admin" role and one administrator account on first start-up.
// Idempotent: running it again does nothing, so it is safe on every start-up.
public static class DbSeeder
{
    public const string AdminRole = "Admin";

    public static void Seed(IServiceProvider services, IConfiguration configuration)
    {
        // Identity services are scoped, so a scope is created for us by
        // Program.cs and passed in here.
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var email = configuration["Identity:Admin:Email"];
        var password = configuration["Identity:Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            // No credentials configured - nothing to seed. Not an error: a
            // deployed application may deliberately not want a seeded admin.
            return;
        }

        // Identity's API is async only. The rest of the template is sync for
        // simplicity, so here - at start-up, before any request - we block.
        if (!roleManager.RoleExistsAsync(AdminRole).GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole(AdminRole)).GetAwaiter().GetResult();
        }

        var admin = userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                // No e-mail sender is configured, so the account is confirmed
                // straight away.
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(admin, password).GetAwaiter().GetResult();
            if (!result.Succeeded)
            {
                // Usually the password broke the rules configured in Program.cs.
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(
                    $"Could not create the admin user: {errors}");
            }
        }

        if (!userManager.IsInRoleAsync(admin, AdminRole).GetAwaiter().GetResult())
        {
            userManager.AddToRoleAsync(admin, AdminRole).GetAwaiter().GetResult();
        }
    }
}
