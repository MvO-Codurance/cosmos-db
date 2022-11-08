using Microsoft.Azure.Cosmos;
using UrlShortener.Modules.Shortner.Models;

namespace UrlShortener.Modules.Shortner.Services;

public class ShortnerRepository : IShortnerRepository
{
    private readonly Container _container;

    public ShortnerRepository(
        ShortnerSettings settings,
        CosmosClient cosmosClient)
    {
        _container = cosmosClient.GetContainer(settings.DatabaseName, settings.ContainerName);
    }
    
    public async Task<string> CreateEntry(ShortnerEntry entry)
    {
        ShortnerEntry createdItem = await _container.CreateItemAsync<ShortnerEntry>(
            item: entry,
            partitionKey: new PartitionKey(entry.Key)
        );

        return createdItem.Id;
    }
}