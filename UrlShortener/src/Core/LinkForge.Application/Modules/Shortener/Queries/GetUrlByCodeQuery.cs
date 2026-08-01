namespace LinkForge.Application.Modules.Shortener.Queries;

public record GetUrlByCodeQuery(string Code) : IRequest<string>;
