using EventRegistrationSystem.Domain.Entities;

namespace EventRegistrationSystem.Application.Abstractions;

public interface IJwtProvider
{
    string GenerateToken(User user, IList<string> roles);
    string GenerateRefreshToken();
}
