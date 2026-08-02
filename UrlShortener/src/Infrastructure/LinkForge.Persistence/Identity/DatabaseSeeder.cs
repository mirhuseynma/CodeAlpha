namespace LinkForge.Persistence.Identity;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        Microsoft.AspNetCore.Identity.RoleManager<Role> roleManager, 
        Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager, configuration);
    }

    private static async Task SeedRolesAsync(Microsoft.AspNetCore.Identity.RoleManager<Role> roleManager)
    {
        // Define standard roles
        var adminRole = new Role { Name = "Admin" };
        var userRole = new Role { Name = "User" };

        if (!await roleManager.RoleExistsAsync(adminRole.Name))
        {
            await roleManager.CreateAsync(adminRole);
            
            // Add all permissions to Admin
            var allPermissions = GetAllPermissions();
            foreach (var permission in allPermissions)
            {
                await roleManager.AddClaimAsync(adminRole, new System.Security.Claims.Claim("Permission", permission));
            }
        }

        if (!await roleManager.RoleExistsAsync(userRole.Name))
        {
            await roleManager.CreateAsync(userRole);

            // Add specific permissions to User
            var userPermissions = new[]
            {
                LinkForge.Application.Common.Constants.Permissions.ShortLinks.Create,
                LinkForge.Application.Common.Constants.Permissions.ShortLinks.View,
                LinkForge.Application.Common.Constants.Permissions.Analytics.View
            };

            foreach (var permission in userPermissions)
            {
                await roleManager.AddClaimAsync(userRole, new System.Security.Claims.Claim("Permission", permission));
            }
        }
    }

    private static async Task SeedAdminUserAsync(Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        var adminEmail = configuration["AdminSettings:Email"] ?? throw new InvalidOperationException("AdminSettings:Email is not configured.");
        var adminPassword = configuration["AdminSettings:Password"] ?? throw new InvalidOperationException("AdminSettings:Password is not configured.");

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    private static IEnumerable<string> GetAllPermissions()
    {
        var permissions = new List<string>();
        var type = typeof(LinkForge.Application.Common.Constants.Permissions);
        var nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var nestedType in nestedTypes)
        {
            var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (var field in fields)
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    var value = field.GetRawConstantValue() as string;
                    if (value != null)
                    {
                        permissions.Add(value);
                    }
                }
            }
        }

        return permissions;
    }
}
