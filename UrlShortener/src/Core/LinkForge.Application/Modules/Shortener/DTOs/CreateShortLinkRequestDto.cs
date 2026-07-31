namespace LinkForge.Application.Modules.Shortener.DTOs;

public class CreateShortLinkRequestDto
{
    public string OriginalUrl { get; set; } = null!;
    public string? CustomAlias { get; set; }
}
