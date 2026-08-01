namespace LinkForge.Application.Modules.Shortener.Queries;

public class GetUrlByCodeQueryHandler : IRequestHandler<GetUrlByCodeQuery, string>
{
    private readonly IAppDbContext _context;

    public GetUrlByCodeQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(GetUrlByCodeQuery request, CancellationToken cancellationToken)
    {
        var link = await _context.ShortenedUrls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ShortCode == request.Code || x.CustomAlias == request.Code, cancellationToken);

        if (link == null)
            throw new NotFoundException("ShortenedUrl", request.Code);

        return link.OriginalUrl;
    }
}

