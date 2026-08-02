namespace LinkForge.Application.Modules.Auth.Commands;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
{
    private readonly IIdentityService _identityService;

    public ConfirmEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ConfirmEmailAsync(request.Email, request.Token);
        if (!result)
        {
            throw new BadRequestException("Invalid or expired email confirmation token.");
        }
        return true;
    }
}
