using AutoMapper;
using LinkForge.Application.Modules.Shortener.DTOs;
using LinkForge.Domain.Entities;

namespace LinkForge.Application.Modules.Shortener.Mappings;

public class ShortenerProfile : Profile
{
    public ShortenerProfile()
    {
        CreateMap<ShortenedUrl, ShortLinkResponseDto>();
    }
}
