using MediatR;
using Microsoft.EntityFrameworkCore;
using LinkForge.Application.Common.Interfaces;
using LinkForge.Application.Common.Models;

namespace LinkForge.Application.Modules.Shortener.Queries;

public class GetUrlByCodeQueryHandler : IRequestHandler<GetUrlByCodeQuery, Result<string>>
{
    private readonly IAppDbContext _context;

    public GetUrlByCodeQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<string>> Handle(GetUrlByCodeQuery request, CancellationToken cancellationToken)
    {
        var link = await _context.ShortenedUrls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ShortCode == request.Code || x.CustomAlias == request.Code, cancellationToken);

        if (link == null)
            throw new LinkForge.Application.Common.Exceptions.NotFoundException("ShortenedUrl", request.Code);

        return Result<string>.Success(link.OriginalUrl);
    }
}

