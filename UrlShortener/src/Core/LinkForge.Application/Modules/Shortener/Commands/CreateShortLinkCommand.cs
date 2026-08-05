namespace LinkForge.Application.Modules.Shortener.Commands;

public record CreateShortLinkCommand(string OriginalUrl, string? CustomAlias, string BaseUrl, DateTimeOffset? ExpiresAt) : IRequest<ShortLinkResponseDto>;
