namespace LinkForge.Application.Modules.Shortener.DTOs;

public class ShortLinkResponseDto
{
    public string ShortCode { get; set; } = null!;
    public string OriginalUrl { get; set; } = null!;
    public string? CustomAlias { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
