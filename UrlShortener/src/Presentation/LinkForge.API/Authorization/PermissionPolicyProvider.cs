namespace LinkForge.API.Authorization;

public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy == null)
        {
            // If the policy name matches a permission, dynamically create a policy for it.
            // Since we use the permission string itself as the policy name, we just wrap it.
            policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
        }

        return policy;
    }
}
