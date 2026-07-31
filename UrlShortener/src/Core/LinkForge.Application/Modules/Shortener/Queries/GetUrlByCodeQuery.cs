using MediatR;
using LinkForge.Application.Common.Models;

namespace LinkForge.Application.Modules.Shortener.Queries;

public record GetUrlByCodeQuery(string Code) : IRequest<Result<string>>;
