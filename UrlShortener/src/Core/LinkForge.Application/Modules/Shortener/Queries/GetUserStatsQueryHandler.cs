using LinkForge.Application.Common.Exceptions;
using LinkForge.Application.Common.Interfaces;
using LinkForge.Application.Modules.Shortener.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LinkForge.Application.Modules.Shortener.Queries;

public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, UserStatsDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserStatsQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<UserStatsDto> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("User is not authenticated.");

        var isAdmin = _currentUserService.IsAdmin;

        var query = _context.ShortenedUrls
            .AsNoTracking()
            .Where(x => isAdmin || x.UserId == userId);

        var activeLinks = await query.CountAsync(cancellationToken);
        var totalClicks = await query.SelectMany(x => x.Visits).CountAsync(cancellationToken);

        return new UserStatsDto(totalClicks, activeLinks);
    }
}