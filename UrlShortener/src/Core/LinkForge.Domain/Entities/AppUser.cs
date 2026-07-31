using Microsoft.AspNetCore.Identity;
using LinkForge.Domain.Common;

namespace LinkForge.Domain.Entities;

public class AppUser : IdentityUser<Guid>, IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<ShortenedUrl> ShortenedUrls { get; set; } = new List<ShortenedUrl>();
}

