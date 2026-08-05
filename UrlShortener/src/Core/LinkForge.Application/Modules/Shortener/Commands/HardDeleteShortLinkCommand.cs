using MediatR;
using System;

namespace LinkForge.Application.Modules.Shortener.Commands;

public record HardDeleteShortLinkCommand(Guid Id) : IRequest;
