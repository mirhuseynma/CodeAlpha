using MediatR;
using LinkForge.Application.Common.Models;

namespace LinkForge.Application.Modules.Shortener.Commands;

public record CreateShortLinkCommand : IRequest<Result<string>>
{
    public string OriginalUrl { get; init; } = string.Empty;
    public string? CustomAlias { get; init; }
}
