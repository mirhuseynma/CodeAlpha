namespace LinkForge.Application.Modules.Auth.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterResponseDto>
{
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<RegisterResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
        {
            throw new BadRequestException("Password and Confirm Password do not match.");
        }

        var (result, userId, emailConfirmationToken, errors) = await _identityService.CreateUserAsync(request.Email, request.Password, request.FullName);
        
        if (!result)
        {
            var errorMsg = errors != null && errors.Any() 
                ? string.Join("; ", errors) 
                : "User registration failed.";
            throw new BadRequestException(errorMsg);
        }

        return new RegisterResponseDto(emailConfirmationToken);
    }
}
