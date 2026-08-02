namespace LinkForge.Application.Modules.Shortener.Commands;

public class CreateShortLinkCommandHandler : IRequestHandler<CreateShortLinkCommand, string>
{
    private readonly IAppDbContext _context;
    private readonly IUrlShorteningService _urlShorteningService;
    private readonly ICurrentUserService _currentUserService;

    public CreateShortLinkCommandHandler(IAppDbContext context, IUrlShorteningService urlShorteningService, ICurrentUserService currentUserService)
    {
        _context = context;
        _urlShorteningService = urlShorteningService;
        _currentUserService = currentUserService;
    }

    public async Task<string> Handle(CreateShortLinkCommand request, CancellationToken cancellationToken)
    {
        var shortCode = request.CustomAlias ?? _urlShorteningService.GenerateShortCode();

        if (request.CustomAlias != null)
        {
            var exists = await _context.ShortenedUrls.AnyAsync(x => x.CustomAlias == request.CustomAlias || x.ShortCode == request.CustomAlias, cancellationToken);
            if (exists)
            {
                throw new BadRequestException("This custom alias is already in use.");
            }
        }
        else
        {
            // Simple loop to handle very rare collisions
            while (await _context.ShortenedUrls.AnyAsync(x => x.ShortCode == shortCode, cancellationToken))
            {
                shortCode = _urlShorteningService.GenerateShortCode();
            }
        }

        var shortenedUrl = new ShortenedUrl
        {
            OriginalUrl = request.OriginalUrl,
            ShortCode = shortCode,
            CustomAlias = request.CustomAlias,
            UserId = _currentUserService.UserId
        };

        _context.ShortenedUrls.Add(shortenedUrl);

        var retryCount = 0;
        var maxRetries = 3;
        
        while (true)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (DbUpdateException)
            {
                if (request.CustomAlias != null)
                {
                    throw new BadRequestException("This custom alias is already in use.");
                }

                retryCount++;
                if (retryCount > maxRetries)
                {
                    throw new Exception("Could not generate a unique short code after multiple attempts.");
                }

                // Generate a new code and try again
                shortenedUrl.ShortCode = _urlShorteningService.GenerateShortCode();
                _context.ShortenedUrls.Update(shortenedUrl);
            }
        }

        return shortenedUrl.ShortCode;
    }
}

