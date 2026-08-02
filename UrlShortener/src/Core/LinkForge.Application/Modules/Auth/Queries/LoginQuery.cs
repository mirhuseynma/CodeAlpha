namespace LinkForge.Application.Modules.Auth.Queries;

public record LoginQuery(string Email, string Password) : IRequest<AuthResponseDto>;
