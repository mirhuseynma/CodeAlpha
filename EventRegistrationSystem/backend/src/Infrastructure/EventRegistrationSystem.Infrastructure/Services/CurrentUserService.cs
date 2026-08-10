
namespace EventRegistrationSystem.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) 
                             ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

    public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.HasClaim(ClaimTypes.Role, "Admin") ?? false;
    
    public bool IsOrganizer => _httpContextAccessor.HttpContext?.User?.HasClaim(ClaimTypes.Role, "Organizer") ?? false;
}
