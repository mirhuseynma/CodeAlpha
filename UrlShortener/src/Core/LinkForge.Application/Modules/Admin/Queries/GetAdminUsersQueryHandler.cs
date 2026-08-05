using LinkForge.Application.Common.Interfaces;
using LinkForge.Application.Common.Interfaces.Identity;
using LinkForge.Application.Common.Pagination;
using LinkForge.Application.Modules.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LinkForge.Application.Modules.Admin.Queries;

public record GetAdminUsersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<AdminUserDto>>;

public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, PagedResult<AdminUserDto>>
{
    private readonly IIdentityService _identityService;

    public GetAdminUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<PagedResult<AdminUserDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _identityService.GetAllUsersAsync();
        
        var totalCount = users.Count();
        
        var paginatedUsers = users
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<AdminUserDto>
        {
            Items = paginatedUsers,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)System.Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}
