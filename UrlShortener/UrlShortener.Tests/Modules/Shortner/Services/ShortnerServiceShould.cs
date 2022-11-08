using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using UrlShortener.Modules.Shortner.Models;
using UrlShortener.Modules.Shortner.Services;
using Xunit;

namespace UrlShortener.Tests.Modules.Shortner.Services;

public class ShortnerServiceShould
{
    [Theory]
    [InlineAutoNSubstituteData("https://www.google.co.uk/")]
    public async Task Return_A_Key_Of_The_Configured_Length(
        string url,
        [Frozen] UrlShortnerSettings settings,
        [Frozen] IKeyGenerator keyGenerator,
        ShortnerService sut)
    {
        var expectedKey = new string('X', settings.KeyLength);
        keyGenerator.CreateKey(Arg.Any<int>()).Returns(expectedKey);

        var actualKey = await sut.CreateShortenedUrl(url);
        
        actualKey.Should().Be(expectedKey);
        keyGenerator.Received(1).CreateKey(settings.KeyLength);
    }
    
    [Theory]
    [InlineAutoNSubstituteData("https://www.google.co.uk/")]
    public async Task Create_A_Shortened_Url_Entry(
        string url,
        string expectedKey,
        string expectedId,
        [Frozen] IKeyGenerator keyGenerator,
        [Frozen] IShortnerRepository repository,
        ShortnerService sut)
    {
        ShortnerEntry entry = new(expectedKey, url);
        
        keyGenerator.CreateKey(Arg.Any<int>()).Returns(expectedKey);
        repository.CreateEntry(Arg.Any<ShortnerEntry>()).Returns(expectedId);

        var actualKey = await sut.CreateShortenedUrl(url);
        
        actualKey.Should().Be(expectedKey);
        await repository.Received(1).CreateEntry(Arg.Is<ShortnerEntry>(
            x => x.Key == expectedKey && x.Url == url));
    }
}
