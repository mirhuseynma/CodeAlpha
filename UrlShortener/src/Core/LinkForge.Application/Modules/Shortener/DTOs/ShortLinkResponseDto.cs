namespace LinkForge.Application.Modules.Shortener.DTOs;

public record ShortLinkResponseDto(string ShortCode, string OriginalUrl, string? CustomAlias, DateTimeOffset CreatedAt);
