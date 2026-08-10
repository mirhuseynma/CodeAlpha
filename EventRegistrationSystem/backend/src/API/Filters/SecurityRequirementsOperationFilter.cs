
namespace EventRegistrationSystem.API.Filters;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true ||
                           context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true;

        if (hasAuthorize)
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();
            
            var requirement = new OpenApiSecurityRequirement();
            requirement.Add(new OpenApiSecuritySchemeReference("Bearer"), new List<string>());
            
            operation.Security.Add(requirement);
        }
    }
}
