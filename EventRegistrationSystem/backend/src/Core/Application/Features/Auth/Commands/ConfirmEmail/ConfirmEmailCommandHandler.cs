namespace EventRegistrationSystem.Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
{
    private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

    public ConfirmEmailCommandHandler(Microsoft.AspNetCore.Identity.UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new EventRegistrationSystem.Application.Exceptions.NotFoundException("User not found.");
        }

        // We use the token directly since it's passed via JSON body
        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new EventRegistrationSystem.Application.Exceptions.BadRequestException($"Email confirmation failed: {errors}");
        }

        return true;
    }
}
