using LinkForge.Application.Common.Interfaces;

namespace LinkForge.API.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId => null; // TODO: Extract from HttpContext.User
}
