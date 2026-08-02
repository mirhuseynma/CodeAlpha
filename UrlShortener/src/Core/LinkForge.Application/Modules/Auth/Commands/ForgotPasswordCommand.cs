namespace LinkForge.Application.Modules.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<string?>;
