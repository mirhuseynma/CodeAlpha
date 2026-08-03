namespace LinkForge.Application.Modules.Shortener.Commands;

public class DeleteShortLinkCommandHandler : IRequestHandler<DeleteShortLinkCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteShortLinkCommandHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteShortLinkCommand request, CancellationToken cancellationToken)
    {
        var link = await _context.ShortenedUrls
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (link == null)
            throw new NotFoundException("ShortenedUrl", request.Id);

        if (link.UserId != _currentUserService.UserId && !_currentUserService.IsAdmin)
            throw new UnauthorizedException("You do not have permission to delete this link.");

        _context.ShortenedUrls.Remove(link);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
