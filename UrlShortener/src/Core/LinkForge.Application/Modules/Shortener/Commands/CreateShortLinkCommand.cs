namespace LinkForge.Application.Modules.Shortener.Commands;

public record CreateShortLinkCommand : IRequest<string>
{
    public string OriginalUrl { get; init; } = string.Empty;
    public string? CustomAlias { get; init; }
}
