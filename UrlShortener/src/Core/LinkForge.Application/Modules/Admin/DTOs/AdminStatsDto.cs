namespace LinkForge.Application.Modules.Admin.DTOs;

public record AdminStatsDto(
    int TotalUsers,
    int TotalLinks,
    int TotalClicks
);
