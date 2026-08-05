namespace LinkForge.Application.Modules.Shortener.Queries;

public class GetLinkByCodeQueryHandler : IRequestHandler<GetLinkByCodeQuery, ShortLinkDetailsDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetLinkByCodeQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ShortLinkDetailsDto> Handle(GetLinkByCodeQuery request, CancellationToken cancellationToken)
    {
        var link = await _context.ShortenedUrls
            .AsNoTracking()
            .Include(x => x.Visits)
            .FirstOrDefaultAsync(x => x.ShortCode == request.Code || x.CustomAlias == request.Code, cancellationToken);

        if (link == null)
            throw new NotFoundException("ShortenedUrl", request.Code);

        var userId = _currentUserService.UserId;
        var isAdmin = _currentUserService.IsAdmin;

        if (link.UserId != userId && !isAdmin)
            throw new UnauthorizedException("You do not have permission to view the details of this link.");

        var recentVisits = link.Visits
            .OrderByDescending(v => v.CreatedAt)
            .Take(50)
            .Select(v => new UrlVisitDto(v.IpAddress, v.Country, v.UserAgent, v.Referer, v.CreatedAt))
            .ToList();

        string shortUrl = request.BaseUrl + link.ShortCode;

        return new ShortLinkDetailsDto(
            link.Id,
            shortUrl,
            link.ShortCode, 
            link.OriginalUrl, 
            link.CustomAlias, 
            link.CreatedAt,
            link.Visits.Count,
            recentVisits,
            link.IsActive,
            link.ExpiresAt
        );
    }
}
