using EventRegistrationSystem.Application.Features.Registrations.Commands.CancelRegistration;
using FluentValidation;

namespace EventRegistrationSystem.Application.Features.Registrations.Commands;

public class CancelRegistrationCommandValidator : AbstractValidator<CancelRegistrationCommand>
{
    public CancelRegistrationCommandValidator()
    {
        RuleFor(v => v.RegistrationId)
            .NotEmpty().WithMessage("RegistrationId is required.");
    }
}
