using FluentAssertions;
using Microsoft.Azure.Cosmos;
using NSubstitute;
using UrlShortener.Modules.Shortner.Models;
using UrlShortener.Modules.Shortner.Services;
using Xunit;

namespace UrlShortener.Tests.Modules.Shortner.Services;

public class ShortnerRepositoryShould
{
    [Theory]
    [InlineAutoNSubstituteData]
    public async Task Create_A_New_Shortner_Entry(
        ShortnerEntry entry,
        Container container,
        ShortnerSettings settings)
    {
        var cosmosClient = Substitute.For<CosmosClient>(
            "https://localhost:8081", 
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", 
            null as CosmosClientOptions);
        var sut = new ShortnerRepository(settings, cosmosClient);
        var createdEntry = Substitute.For<ItemResponse<ShortnerEntry>>();
        
        cosmosClient.GetContainer(Arg.Any<string>(), Arg.Any<string>()).Returns(container);
        container.CreateItemAsync<ShortnerEntry>(
            item: entry,
            partitionKey: new PartitionKey(entry.Key)
        ).Returns(Task.FromResult(createdEntry));

        var actual = await sut.CreateEntry(entry);

        actual.Should().NotBeNull();
        actual.Should().Be(entry.Id);
    }
}