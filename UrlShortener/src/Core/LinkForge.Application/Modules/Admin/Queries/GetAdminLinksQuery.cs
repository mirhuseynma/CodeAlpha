using LinkForge.Application.Common.Pagination;
using LinkForge.Application.Modules.Admin.DTOs;
using MediatR;

namespace LinkForge.Application.Modules.Admin.Queries;

public record GetAdminLinksQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<AdminLinkDto>>;
