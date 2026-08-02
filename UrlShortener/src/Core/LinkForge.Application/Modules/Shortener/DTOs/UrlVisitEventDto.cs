namespace LinkForge.Application.Modules.Shortener.DTOs;

public record UrlVisitEventDto(
    Guid ShortenedUrlId, 
    string? IpAddress, 
    string? UserAgent, 
    string? Referer);
