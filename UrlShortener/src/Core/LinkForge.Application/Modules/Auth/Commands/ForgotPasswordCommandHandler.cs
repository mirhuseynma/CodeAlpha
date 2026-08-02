namespace LinkForge.Application.Modules.Auth.Commands;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string?>
{
    private readonly IIdentityService _identityService;

    public ForgotPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string?> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var token = await _identityService.GeneratePasswordResetTokenAsync(request.Email);
        if (token == null)
        {
            throw new NotFoundException($"User with email {request.Email} not found.");
        }
        return token;
    }
}
