namespace LinkForge.Application.Modules.Shortener.DTOs;

public record CreateShortLinkRequestDto(string OriginalUrl, string? CustomAlias);
