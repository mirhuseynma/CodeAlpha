using LinkForge.Application.Modules.Admin.DTOs;
using MediatR;

namespace LinkForge.Application.Modules.Admin.Queries;

public record GetAdminStatsQuery() : IRequest<AdminStatsDto>;
