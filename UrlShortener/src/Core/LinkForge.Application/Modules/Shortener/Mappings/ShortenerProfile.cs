namespace LinkForge.Application.Modules.Shortener.Mappings;

public class ShortenerProfile : Profile
{
    public ShortenerProfile()
    {
        CreateMap<ShortenedUrl, ShortLinkResponseDto>();
    }
}
