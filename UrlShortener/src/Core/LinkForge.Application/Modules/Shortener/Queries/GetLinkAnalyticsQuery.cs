using LinkForge.Application.Common.Pagination;
using LinkForge.Application.Modules.Shortener.DTOs;
using MediatR;
using System;

namespace LinkForge.Application.Modules.Shortener.Queries;

public record GetLinkAnalyticsQuery(Guid LinkId, int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<UrlVisitDto>>;
