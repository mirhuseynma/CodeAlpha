namespace LinkForge.Application.Modules.Shortener.Queries;

public record GetUserLinksQuery(int PageNumber, int PageSize, string BaseUrl) : IRequest<PagedResult<ShortLinkResponseDto>>;
