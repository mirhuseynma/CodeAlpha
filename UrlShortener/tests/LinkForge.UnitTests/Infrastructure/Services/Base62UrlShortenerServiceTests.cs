using LinkForge.Infrastructure.Services;
using Xunit;

namespace LinkForge.UnitTests.Infrastructure.Services;

public class Base62UrlShortenerServiceTests
{
    private readonly Base62UrlShortenerService _sut;

    public Base62UrlShortenerServiceTests()
    {
        _sut = new Base62UrlShortenerService();
    }

    [Fact]
    public void GenerateShortCode_ShouldReturnExactly7Characters()
    {
        // Act
        var result = _sut.GenerateShortCode();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result));
        Assert.Equal(7, result.Length);
    }
}
