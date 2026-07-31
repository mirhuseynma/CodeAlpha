using System.Text;

namespace LinkForge.Infrastructure.Services;



public class Base62UrlShortenerService : IUrlShorteningService
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private readonly Random _random = new();

    public string GenerateShortCode()
    {
        // 7 character random short code
        var length = 7;
        var sb = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            var index = _random.Next(Alphabet.Length);
            sb.Append(Alphabet[index]);
        }

        return sb.ToString();
    }
}
