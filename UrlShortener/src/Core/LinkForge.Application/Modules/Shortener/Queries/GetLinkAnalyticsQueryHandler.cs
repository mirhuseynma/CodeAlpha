using LinkForge.Application.Common.Exceptions;
using LinkForge.Application.Common.Interfaces;
using LinkForge.Application.Common.Pagination;
using LinkForge.Application.Modules.Shortener.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LinkForge.Application.Modules.Shortener.Queries;

public class GetLinkAnalyticsQueryHandler : IRequestHandler<GetLinkAnalyticsQuery, PagedResult<UrlVisitDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetLinkAnalyticsQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<UrlVisitDto>> Handle(GetLinkAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var link = await _context.ShortenedUrls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.LinkId, cancellationToken);

        if (link == null)
            throw new NotFoundException("ShortenedUrl", request.LinkId);

        if (link.UserId != _currentUserService.UserId && !_currentUserService.IsAdmin)
            throw new UnauthorizedException("You do not have permission to view analytics for this link.");

        var query = _context.UrlVisits
            .AsNoTracking()
            .Where(x => x.ShortenedUrlId == request.LinkId)
            .OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var visits = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => new UrlVisitDto(v.IpAddress, v.Country, v.UserAgent, v.Referer, v.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<UrlVisitDto>
        {
            Items = visits,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)System.Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}
