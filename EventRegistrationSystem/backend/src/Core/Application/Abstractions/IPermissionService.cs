namespace EventRegistrationSystem.Application.Abstractions;

public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionsAsync(string userId);
}
