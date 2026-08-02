namespace LinkForge.Application.Modules.Auth.Commands;

public record RegisterUserCommand(string Email, string Password, string ConfirmPassword, string FullName) : IRequest<RegisterResponseDto>;
