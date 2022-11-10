using System.Net;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Moq;
using UrlShortener.Modules.Shortner.Models;
using UrlShortener.Modules.Shortner.Services;
using Xunit;

namespace UrlShortener.Tests.Modules.Shortner.Services;

public class MockedShortnerRepositoryShould
{
    [Theory]
    [InlineAutoMoqData]
    public async Task Create_A_New_Shortner_Entry(
        ShortnerSettings settings,
        ShortnerEntry entry)
    {
        // arrange
        var itemResponse = new Mock<ItemResponse<ShortnerEntry>>();
        itemResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
        itemResponse.SetupGet(x => x.Resource).Returns(entry);
        
        var container = new Mock<Container>();
        container.Setup(x => x.CreateItemAsync(
                It.IsAny<ShortnerEntry>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemResponse.Object);
        
        var cosmosClient = new Mock<CosmosClient>();
        cosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);
        
        var sut = new ShortnerRepository(settings, cosmosClient.Object);
        
        // act
        var actualId = await sut.CreateEntry(entry);

        // assert
        actualId.Should().NotBeNull();
        actualId.Should().Be(entry.Id);
        
        container.Verify(x => x.CreateItemAsync(
                entry, 
                It.Is<PartitionKey>(pk => pk.ToString() == $"""["{entry.Id}"]"""),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Theory]
    [InlineAutoMoqData]
    public async Task Retrieve_A_Previously_Created_Entry_Using_The_Id(
        ShortnerSettings settings,
        ShortnerEntry entry)
    {
        // arrange
        var itemResponse = new Mock<ItemResponse<ShortnerEntry>>();
        itemResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
        itemResponse.SetupGet(x => x.Resource).Returns(entry);
        
        var container = new Mock<Container>();
        container.Setup(x => x.CreateItemAsync(
                It.IsAny<ShortnerEntry>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemResponse.Object);

        container.Setup(x => x.ReadItemAsync<ShortnerEntry>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemResponse.Object);
        
        var cosmosClient = new Mock<CosmosClient>();
        cosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);
        
        var sut = new ShortnerRepository(settings, cosmosClient.Object);
        
        var storedEntryId = await sut.CreateEntry(entry);
        storedEntryId.Should().NotBeNull();
    
        // act
        var actualEntry = await sut.GetEntry(entry.Id);
        
        // assert
        actualEntry.Should().BeEquivalentTo(entry);
        
        container.Verify(x => x.ReadItemAsync<ShortnerEntry>(
                entry.Id, 
                It.Is<PartitionKey>(pk => pk.ToString() == $"""["{entry.Id}"]"""),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Theory]
    [InlineAutoMoqData]
    public async Task Retrieve_Null_For_An_Unknown_Id(
        ShortnerSettings settings,
        ShortnerEntry entry)
    {
        // arrange
        var container = new Mock<Container>();
        container.Setup(x => x.ReadItemAsync<ShortnerEntry>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException(
                message: string.Empty, 
                statusCode: HttpStatusCode.NotFound,
                subStatusCode: 0,
                activityId: string.Empty,
                requestCharge: 0.0d));
        
        var cosmosClient = new Mock<CosmosClient>();
        cosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);
        
        var sut = new ShortnerRepository(settings, cosmosClient.Object);
    
        // act
        var actualEntry = await sut.GetEntry(entry.Id);
        
        // assert
        actualEntry.Should().BeNull();
        
        container.Verify(x => x.ReadItemAsync<ShortnerEntry>(
                entry.Id, 
                It.Is<PartitionKey>(pk => pk.ToString() == $"""["{entry.Id}"]"""),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}