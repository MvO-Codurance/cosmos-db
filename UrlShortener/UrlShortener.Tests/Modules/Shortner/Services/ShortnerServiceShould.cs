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
        [Frozen] ShortnerSettings settings,
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
        Guid expectedId,
        [Frozen] IKeyGenerator keyGenerator,
        [Frozen] IShortnerRepository repository,
        ShortnerService sut)
    {
        keyGenerator.CreateKey(Arg.Any<int>()).Returns(expectedKey);
        repository.CreateEntry(Arg.Any<ShortnerEntry>()).Returns(expectedId.ToString());

        var actualKey = await sut.CreateShortenedUrl(url);
        
        actualKey.Should().Be(expectedKey);
        await repository.Received(1).CreateEntry(Arg.Is<ShortnerEntry>(
            x => x.Key == expectedKey && x.Url == url));
    }
    
    [Theory]
    [InlineAutoNSubstituteData]
    public async Task Return_The_Correct_Url_For_A_Previously_Created_Key(
        ShortnerEntry entry,
        [Frozen] IShortnerRepository repository,
        ShortnerService sut)
    {
        repository.GetEntry(Arg.Any<string>()).Returns(entry);

        var actualUrl = await sut.GetOriginalUrl(entry.Key);
        
        actualUrl.Should().Be(entry.Url);
        await repository.Received(1).GetEntry(entry.Key);
    }
}
