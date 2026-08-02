namespace LinkForge.Application.Modules.Auth.Commands;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<bool>;
