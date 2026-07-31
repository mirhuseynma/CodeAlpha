namespace LinkForge.Domain.Entities;

public class ShortenedUrl : BaseAuditableEntity
{
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    
    // Optional custom alias provided by AppUser
    public string? CustomAlias { get; set; } 
    
    public DateTimeOffset? ExpiresAt { get; set; }

    public Guid? UserId { get; set; }
    public AppUser? AppUser { get; set; }

    public ICollection<UrlVisit> Visits { get; set; } = new List<UrlVisit>();
}

