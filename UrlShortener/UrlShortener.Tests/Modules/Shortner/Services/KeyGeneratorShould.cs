using FluentAssertions;
using UrlShortener.Modules.Shortner.Services;
using Xunit;

namespace UrlShortener.Tests.Modules.Shortner.Services;

public class KeyGeneratorShould
{
    [Theory]
    [InlineAutoMoqData(5)]
    [InlineAutoMoqData(6)]
    [InlineAutoMoqData(7)]
    [InlineAutoMoqData(10)]
    [InlineAutoMoqData(20)]
    public void Create_A_Base64_Encoded_Key_With_The_Correct_Length(
        int length,
        KeyGenerator sut)
    {
        var actual = sut.CreateKey(length);

        actual.Should().NotBeNull();
        var converted = Convert.FromBase64String(actual);
        converted.Length.Should().Be(length);
    }
}
