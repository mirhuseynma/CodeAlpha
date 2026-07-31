namespace LinkForge.Domain.Entities;

public class UrlVisit : BaseAuditableEntity
{
    public Guid ShortenedUrlId { get; set; }
    public ShortenedUrl? ShortenedUrl { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Referer { get; set; }
    public string? Country { get; set; }
}
