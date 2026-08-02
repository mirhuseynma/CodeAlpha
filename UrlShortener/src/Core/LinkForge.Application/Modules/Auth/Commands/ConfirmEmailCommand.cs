namespace LinkForge.Application.Modules.Auth.Commands;

public record ConfirmEmailCommand(string Email, string Token) : IRequest<bool>;
