namespace LinkForge.Application.Modules.Shortener.DTOs;

public record ShortLinkDetailsDto(
    Guid Id,
    string ShortUrl,
    string ShortCode, 
    string OriginalUrl, 
    string? CustomAlias, 
    DateTimeOffset CreatedAt,
    int TotalClicks,
    List<UrlVisitDto> RecentVisits,
    bool IsActive
);
