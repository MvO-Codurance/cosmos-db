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

    public async Task<ShortnerEntry?> GetEntry(string key)
    {
        var query = new QueryDefinition(query: "SELECT * FROM c WHERE c.key = @key")
            .WithParameter("@key", key);
        
        using FeedIterator<ShortnerEntry> feed = 
            _container.GetItemQueryIterator<ShortnerEntry>(queryDefinition: query);

        if (feed.HasMoreResults)
        {
            FeedResponse<ShortnerEntry>? response = await feed.ReadNextAsync();
            return response?.FirstOrDefault();
        }

        return null;
    }
}