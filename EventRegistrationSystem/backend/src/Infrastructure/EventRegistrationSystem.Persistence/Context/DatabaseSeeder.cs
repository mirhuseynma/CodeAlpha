namespace EventRegistrationSystem.Persistence.Context;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(RoleManager<Role> roleManager, UserManager<User> userManager, IConfiguration configuration)
    {
        // 1. Seed Roles and Permissions
        var allPermissions = GetAllPermissions().ToArray();
        
        await SeedRoleWithPermissionsAsync(roleManager, "Admin", allPermissions);

        await SeedRoleWithPermissionsAsync(roleManager, "Organizer", new[]
        {
            EventRegistrationSystem.Domain.Constants.Permissions.Events.Create,
            EventRegistrationSystem.Domain.Constants.Permissions.Events.View,
            EventRegistrationSystem.Domain.Constants.Permissions.Events.Update,
            EventRegistrationSystem.Domain.Constants.Permissions.Events.Delete,
            EventRegistrationSystem.Domain.Constants.Permissions.Events.Register,
            EventRegistrationSystem.Domain.Constants.Permissions.Organizers.View
        });

        await SeedRoleWithPermissionsAsync(roleManager, "User", new[]
        {
            EventRegistrationSystem.Domain.Constants.Permissions.Events.View,
            EventRegistrationSystem.Domain.Constants.Permissions.Events.Register,
            EventRegistrationSystem.Domain.Constants.Permissions.Organizers.View
        });

        // 2. Seed Admin User
        var adminEmail = configuration["ADMIN_EMAIL"];
        var adminPassword = configuration["ADMIN_PASSWORD"];

        if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
        {
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
        else
        {
            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (!isDevelopment)
            {
                throw new InvalidOperationException("ADMIN_EMAIL and ADMIN_PASSWORD environment variables are required.");
            }
        }

        // 3. Seed Organizer User
        var organizerEmail = configuration["ORGANIZER_EMAIL"] ?? "organizer@example.com";
        var organizerPassword = configuration["ORGANIZER_PASSWORD"];
        
        if (!string.IsNullOrEmpty(organizerPassword))
        {
            var organizerUser = await userManager.FindByEmailAsync(organizerEmail);
            if (organizerUser == null)
            {
                organizerUser = new User
                {
                    UserName = organizerEmail,
                    Email = organizerEmail,
                    FirstName = "Test",
                    LastName = "Organizer",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(organizerUser, organizerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(organizerUser, "Organizer");
                }
            }
        }
    }

    private static async Task SeedRoleWithPermissionsAsync(RoleManager<Role> roleManager, string roleName, string[] permissions)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            role = new Role { Name = roleName };
            await roleManager.CreateAsync(role);
        }

        var existingClaims = await roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", permission));
            }
        }
    }

    private static IEnumerable<string> GetAllPermissions()
    {
        var permissions = new List<string>();
        var type = typeof(EventRegistrationSystem.Domain.Constants.Permissions);
        var nestedTypes = type.GetNestedTypes(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        foreach (var nestedType in nestedTypes)
        {
            var fields = nestedType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy);
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

