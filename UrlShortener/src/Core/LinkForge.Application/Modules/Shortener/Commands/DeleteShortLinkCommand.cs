namespace LinkForge.Application.Modules.Shortener.Commands;

public record DeleteShortLinkCommand(Guid Id) : IRequest;
