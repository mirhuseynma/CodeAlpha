using LinkForge.Application.Common.Interfaces;
using LinkForge.Application.Common.Pagination;
using LinkForge.Application.Modules.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LinkForge.Application.Modules.Admin.Queries;

public class GetAdminLinksQueryHandler : IRequestHandler<GetAdminLinksQuery, PagedResult<AdminLinkDto>>
{
    private readonly IAppDbContext _context;
    public GetAdminLinksQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminLinkDto>> Handle(GetAdminLinksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ShortenedUrls
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.Visits)
            .OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var links = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(link => new AdminLinkDto(
                link.Id,
                link.OriginalUrl,
                link.ShortCode,
                link.CustomAlias,
                link.Visits.Count,
                link.IsActive,
                link.IsDeleted,
                link.CreatedAt,
                link.UserId.ToString(), // We will return UserId here temporarily
                link.ExpiresAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminLinkDto>
        {
            Items = links,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)System.Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}
