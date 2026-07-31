namespace LinkForge.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}

