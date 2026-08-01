namespace LinkForge.Persistence.Identity;

public class Role : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
