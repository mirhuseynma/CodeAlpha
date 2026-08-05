namespace LinkForge.Application.Modules.Shortener.Queries;

public class GetUrlByCodeQueryHandler : IRequestHandler<GetUrlByCodeQuery, RedirectResponseDto>
{
    private readonly IAppDbContext _context;
    private readonly ICacheService _cacheService;

    public GetUrlByCodeQueryHandler(IAppDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<RedirectResponseDto> Handle(GetUrlByCodeQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"UrlShortener:{request.Code}";
        var cachedData = await _cacheService.GetAsync<RedirectResponseDto>(cacheKey, cancellationToken);
        if (cachedData != null)
        {
            return cachedData;
        }

        var link = await _context.ShortenedUrls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ShortCode == request.Code || x.CustomAlias == request.Code, cancellationToken);

        if (link == null || link.IsDeleted)
            throw new NotFoundException("ShortenedUrl", request.Code);
            
        if (!link.IsActive)
            throw new BadRequestException("This link has been deactivated.");

        if (link.ExpiresAt.HasValue && link.ExpiresAt.Value < DateTimeOffset.UtcNow)
            throw new BadRequestException("This link has expired.");

        var response = new RedirectResponseDto(link.Id, link.OriginalUrl);
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromHours(1), cancellationToken);

        return response;
    }
}
