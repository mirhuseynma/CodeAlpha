namespace LinkForge.Application.Modules.Shortener.Queries;

public class GetUserLinksQueryHandler : IRequestHandler<GetUserLinksQuery, PagedResult<ShortLinkResponseDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserLinksQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<ShortLinkResponseDto>> Handle(GetUserLinksQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("User is not authenticated.");
        
        var isAdmin = _currentUserService.IsAdmin;

        var query = _context.ShortenedUrls
            .AsNoTracking()
            .Where(x => isAdmin || x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ShortLinkResponseDto(
                x.Id,
                request.BaseUrl + x.ShortCode,
                x.ShortCode, 
                x.OriginalUrl, 
                x.CustomAlias, 
                x.CreatedAt,
                x.Visits.Count(),
                x.IsActive,
                x.ExpiresAt
            ));

        return await PagedResult<ShortLinkResponseDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
    }
}
