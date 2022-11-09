using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using UrlShortener.Modules.Shortner.Models;
using UrlShortener.Modules.Shortner.Services;
using Xunit;

namespace UrlShortener.Tests.Modules.Shortner.Services;

public class ShortnerServiceShould
{
    [Theory]
    [InlineAutoMoqData("https://www.google.co.uk/")]
    public async Task Return_A_Key_Of_The_Configured_Length(
        string url,
        [Frozen] ShortnerSettings settings,
        [Frozen] Mock<IKeyGenerator> keyGenerator,
        ShortnerService sut)
    {
        var expectedKey = new string('X', settings.KeyLength);
        keyGenerator.Setup(x => x.CreateKey(It.IsAny<int>()))
            .Returns(expectedKey);

        var actualKey = await sut.CreateShortenedUrl(url);
        
        actualKey.Should().Be(expectedKey);
        keyGenerator.Verify(x => x.CreateKey(settings.KeyLength), Times.Once);
    }
    
    [Theory]
    [InlineAutoMoqData("https://www.google.co.uk/")]
    public async Task Create_A_Shortened_Url_Entry(
        string url,
        string expectedId,
        [Frozen] Mock<IKeyGenerator> keyGenerator,
        [Frozen] Mock<IShortnerRepository> repository,
        ShortnerService sut)
    {
        keyGenerator.Setup(x => x.CreateKey(It.IsAny<int>()))
            .Returns(expectedId);
        
        repository.Setup(x => x.CreateEntry(It.IsAny<ShortnerEntry>()))
            .ReturnsAsync(expectedId);

        var actualKey = await sut.CreateShortenedUrl(url);
        
        actualKey.Should().Be(expectedId);
        repository.Verify(x => x.CreateEntry(
            It.Is<ShortnerEntry>(entry => entry.Id == expectedId && entry.Url == url)),
            Times.Once);
        
    }
    
    [Theory]
    [InlineAutoMoqData]
    public async Task Return_The_Correct_Url_For_A_Previously_Created_Key(
        ShortnerEntry entry,
        [Frozen] Mock<IShortnerRepository> repository,
        ShortnerService sut)
    {
        repository.Setup(x => x.GetEntry(It.IsAny<string>()))
            .ReturnsAsync(entry);

        var actualUrl = await sut.GetOriginalUrl(entry.Id);
        
        actualUrl.Should().Be(entry.Url);
        repository.Verify(x => x.GetEntry(entry.Id), Times.Once);
    }
}
