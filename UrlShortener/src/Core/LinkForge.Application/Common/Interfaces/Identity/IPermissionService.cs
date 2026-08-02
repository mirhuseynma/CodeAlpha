namespace LinkForge.Application.Common.Interfaces.Identity;

public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken = default);
}
