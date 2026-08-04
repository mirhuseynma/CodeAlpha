namespace LinkForge.Application.Modules.Shortener.DTOs;

public record UserStatsDto(
    int TotalClicks,
    int ActiveLinks
);