namespace LinkForge.Persistence.Identity;

public class PermissionService : IPermissionService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IMemoryCache _cache;

    public PermissionService(
        UserManager<AppUser> userManager,
        RoleManager<Role> roleManager,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _cache = cache;
    }

    public async Task<HashSet<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"permissions_{userId}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return [];
            }

            var permissions = new HashSet<string>();
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    var rolePermissions = roleClaims
                        .Where(c => c.Type == "Permission")
                        .Select(c => c.Value);
                    
                    foreach (var permission in rolePermissions)
                    {
                        permissions.Add(permission);
                    }
                }
            }

            return permissions;
        }) ?? [];
    }
}
