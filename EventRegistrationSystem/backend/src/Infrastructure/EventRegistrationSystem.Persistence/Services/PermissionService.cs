
namespace EventRegistrationSystem.Persistence.Services;

public class PermissionService : IPermissionService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public PermissionService(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<HashSet<string>> GetPermissionsAsync(string userId)
    {
        var permissions = new HashSet<string>();
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return permissions;

        // Get User Roles
        var userRoles = await _userManager.GetRolesAsync(user);

        // Get claims for each role
        foreach (var roleName in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in roleClaims.Where(c => c.Type == "Permission"))
                {
                    permissions.Add(claim.Value);
                }
            }
        }

        return permissions;
    }
}
