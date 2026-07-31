using FluentValidation;

namespace LinkForge.Application.Modules.Shortener.Commands;

public class CreateShortLinkCommandValidator : AbstractValidator<CreateShortLinkCommand>
{
    public CreateShortLinkCommandValidator()
    {
        RuleFor(v => v.OriginalUrl)
            .NotEmpty().WithMessage("Original URL is required.")
            .Must(BeAValidUrl).WithMessage("Must be a valid URL.")
            .MaximumLength(2048);

        RuleFor(v => v.CustomAlias)
            .MaximumLength(20)
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("Custom alias can only contain alphanumeric characters, underscores, and dashes.")
            .When(v => !string.IsNullOrEmpty(v.CustomAlias));
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var outUri) 
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
    }
}
