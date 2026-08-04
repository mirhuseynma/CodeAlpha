using LinkForge.Application.Modules.Shortener.DTOs;
using MediatR;

namespace LinkForge.Application.Modules.Shortener.Queries;

public record GetUserStatsQuery() : IRequest<UserStatsDto>;