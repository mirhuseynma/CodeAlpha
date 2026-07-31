using MediatR;
using LinkForge.Application.Common.Interfaces;
using LinkForge.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkForge.Application.Modules.Shortener.Commands;

public class CreateShortLinkCommandHandler : IRequestHandler<CreateShortLinkCommand, Result<string>>
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

    public async Task<Result<string>> Handle(CreateShortLinkCommand request, CancellationToken cancellationToken)
    {
        var shortCode = request.CustomAlias ?? _urlShorteningService.GenerateShortCode();

        if (request.CustomAlias != null)
        {
            var exists = await _context.ShortenedUrls.AnyAsync(x => x.CustomAlias == request.CustomAlias || x.ShortCode == request.CustomAlias, cancellationToken);
            if (exists)
            {
                return Result<string>.Failure("This custom alias is already in use.");
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
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(shortCode);
    }
}

