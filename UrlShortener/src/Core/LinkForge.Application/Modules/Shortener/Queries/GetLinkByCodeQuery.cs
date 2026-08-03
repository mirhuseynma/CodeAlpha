namespace LinkForge.Application.Modules.Shortener.Queries;

public record GetLinkByCodeQuery(string Code, string BaseUrl) : IRequest<ShortLinkDetailsDto>;
