namespace EventRegistrationSystem.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
    private readonly Microsoft.AspNetCore.Identity.RoleManager<Role> _roleManager;

    public RegisterCommandHandler(Microsoft.AspNetCore.Identity.UserManager<User> userManager, Microsoft.AspNetCore.Identity.RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new EventRegistrationSystem.Application.Exceptions.BadRequestException("User with this email already exists."); // TODO: Custom exception
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new EventRegistrationSystem.Application.Exceptions.BadRequestException($"Registration failed: {errors}"); // TODO: Custom exception
        }

        var roleName = "User";
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        return new RegisterResult(user.Id, "Registration successful. Please confirm your email.", token);
    }
}
