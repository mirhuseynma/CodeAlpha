namespace EventRegistrationSystem.Application.Features.Auth.Commands.ConfirmEmail;

public record ConfirmEmailCommand(Guid UserId, string Token) : IRequest<bool>;
