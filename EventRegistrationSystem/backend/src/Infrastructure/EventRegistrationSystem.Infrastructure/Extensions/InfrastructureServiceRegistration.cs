using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace EventRegistrationSystem.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtProvider, EventRegistrationSystem.Infrastructure.Authentication.JwtProvider>();

        var secretKey = configuration["JWT_SECRET"]!;
        var issuer = configuration["JWT_ISSUER"]!;
        var audience = configuration["JWT_AUDIENCE"]!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development";
            options.SaveToken = true;
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddScoped<EventRegistrationSystem.Application.Abstractions.ICurrentUserService, EventRegistrationSystem.Infrastructure.Services.CurrentUserService>();
        return services;
    }
}
