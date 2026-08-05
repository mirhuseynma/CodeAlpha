namespace LinkForge.Application.Modules.Shortener.DTOs;

public record ShortLinkResponseDto(
    Guid Id,
    string ShortUrl,
    string ShortCode, 
    string OriginalUrl, 
    string? CustomAlias, 
    DateTimeOffset CreatedAt,
    int TotalClicks,
    bool IsActive,
    DateTimeOffset? ExpiresAt
);
