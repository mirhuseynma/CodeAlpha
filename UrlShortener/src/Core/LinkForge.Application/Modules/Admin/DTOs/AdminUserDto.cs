using System;

namespace LinkForge.Application.Modules.Admin.DTOs;

public record AdminUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    int LinksCount,
    DateTimeOffset CreatedAt
);
