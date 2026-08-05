using System;

namespace LinkForge.Application.Modules.Admin.DTOs;

public record AdminLinkDto(
    Guid Id,
    string OriginalUrl,
    string ShortCode,
    string? CustomAlias,
    int VisitsCount,
    bool IsActive,
    bool IsDeleted,
    DateTimeOffset CreatedAt,
    string? UserEmail
);
