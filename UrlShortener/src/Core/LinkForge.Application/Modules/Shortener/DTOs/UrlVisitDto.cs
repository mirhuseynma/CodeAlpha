namespace LinkForge.Application.Modules.Shortener.DTOs;

public record UrlVisitDto(string? IpAddress, string? Country, string? UserAgent, string? Referer, DateTimeOffset VisitedAt);
