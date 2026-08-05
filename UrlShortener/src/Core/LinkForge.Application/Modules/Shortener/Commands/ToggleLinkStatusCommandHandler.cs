using LinkForge.Application.Common.Exceptions;
using LinkForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace LinkForge.Application.Modules.Shortener.Commands;

public class ToggleLinkStatusCommandHandler : IRequestHandler<ToggleLinkStatusCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;

    public ToggleLinkStatusCommandHandler(IAppDbContext context, ICurrentUserService currentUserService, ICacheService cacheService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    public async Task Handle(ToggleLinkStatusCommand request, CancellationToken cancellationToken)
    {
        var link = await _context.ShortenedUrls
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (link == null)
            throw new NotFoundException("ShortenedUrl", request.Id);

        if (link.UserId != _currentUserService.UserId && !_currentUserService.IsAdmin)
            throw new UnauthorizedException("You do not have permission to modify this link.");

        link.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
        
        // Remove from cache to apply status immediately
        await _cacheService.RemoveAsync($"UrlShortener:{link.ShortCode}", cancellationToken);
        if (!string.IsNullOrEmpty(link.CustomAlias))
        {
            await _cacheService.RemoveAsync($"UrlShortener:{link.CustomAlias}", cancellationToken);
        }
    }
}
