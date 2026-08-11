using EventRegistrationSystem.Application.Features.Registrations.Commands.RegisterForEvent;
using FluentValidation;

namespace EventRegistrationSystem.Application.Features.Registrations.Commands;

public class RegisterForEventCommandValidator : AbstractValidator<RegisterForEventCommand>
{
    public RegisterForEventCommandValidator()
    {
        RuleFor(v => v.EventId)
            .NotEmpty().WithMessage("EventId is required.");
    }
}
