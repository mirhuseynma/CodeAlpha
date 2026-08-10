
namespace EventRegistrationSystem.API.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;

    public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory, ILogger<PermissionAuthorizationHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            ?? context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userId))
            return;

        using var scope = _scopeFactory.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        var permissions = await permissionService.GetPermissionsAsync(userId);

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("Access denied. User={UserId}, Permission={Permission}", userId, requirement.Permission);
        }
    }
}

