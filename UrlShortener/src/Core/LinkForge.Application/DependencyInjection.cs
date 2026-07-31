using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LinkForge.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LinkForge.Application.Common.Behaviors.ValidationBehavior<,>));
        });

        services.AddAutoMapper(config => 
        {
            config.AddMaps(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
