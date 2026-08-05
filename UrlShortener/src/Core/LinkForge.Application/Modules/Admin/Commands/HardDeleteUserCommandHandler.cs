using LinkForge.Application.Common.Exceptions;
using LinkForge.Application.Common.Interfaces;
using LinkForge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace LinkForge.Application.Modules.Admin.Commands;

public class HardDeleteUserCommandHandler : IRequestHandler<HardDeleteUserCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public HardDeleteUserCommandHandler(IAppDbContext context, ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task Handle(HardDeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAdmin)
            throw new UnauthorizedException("Only administrators can perform hard deletions.");

        // First delete all URLs associated with the user
        await _context.ShortenedUrls
            .IgnoreQueryFilters()
            .Where(x => x.UserId == request.UserId)
            .ExecuteDeleteAsync(cancellationToken);
            
        var success = await _identityService.DeleteUserAsync(request.UserId);
        if (!success)
            throw new NotFoundException("User", request.UserId);
    }
}
