using LinkForge.Application.Common.Exceptions;
using LinkForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace LinkForge.Application.Modules.Shortener.Commands;

public class HardDeleteShortLinkCommandHandler : IRequestHandler<HardDeleteShortLinkCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public HardDeleteShortLinkCommandHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(HardDeleteShortLinkCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAdmin)
            throw new UnauthorizedException("Only administrators can perform hard deletions.");

        var link = await _context.ShortenedUrls
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (link == null)
            throw new NotFoundException("ShortenedUrl", request.Id);

        // Execute raw SQL to hard delete and bypass EF Core change tracker interceptors
        await _context.ShortenedUrls
            .IgnoreQueryFilters()
            .Where(x => x.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
