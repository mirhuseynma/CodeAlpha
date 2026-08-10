
namespace EventRegistrationSystem.Application.Features.Events.Commands.CreateEvent;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
            
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
            
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.");
            
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");
            
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.");
            
        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.");
    }
}
