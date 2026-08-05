using MediatR;
using System;

namespace LinkForge.Application.Modules.Admin.Commands;

public record HardDeleteUserCommand(Guid UserId) : IRequest;
