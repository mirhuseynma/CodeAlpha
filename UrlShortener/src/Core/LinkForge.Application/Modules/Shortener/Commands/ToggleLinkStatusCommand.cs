using MediatR;
using System;

namespace LinkForge.Application.Modules.Shortener.Commands;

public record ToggleLinkStatusCommand(Guid Id, bool IsActive) : IRequest;
