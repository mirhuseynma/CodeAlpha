namespace LinkForge.Application.Common.Interfaces.Identity;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, IList<string> roles);
}
