using LinkForge.Application.Common.Interfaces;
using LinkForge.Application.Common.Interfaces.Identity;
using LinkForge.Application.Modules.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace LinkForge.Application.Modules.Admin.Queries;

public class GetAdminStatsQueryHandler : IRequestHandler<GetAdminStatsQuery, AdminStatsDto>
{
    private readonly IAppDbContext _context;
    private readonly IIdentityService _identityService;

    public GetAdminStatsQueryHandler(IAppDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<AdminStatsDto> Handle(GetAdminStatsQuery request, CancellationToken cancellationToken)
    {
        var totalLinks = await _context.ShortenedUrls.IgnoreQueryFilters().CountAsync(cancellationToken);
        var totalClicks = await _context.UrlVisits.CountAsync(cancellationToken);
        
        int totalUsers = await _identityService.GetTotalUsersAsync();
        
        return new AdminStatsDto(totalUsers, totalLinks, totalClicks);
    }
}
