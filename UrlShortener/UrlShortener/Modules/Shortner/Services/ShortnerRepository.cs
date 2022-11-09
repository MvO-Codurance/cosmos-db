using System.Net;
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
        ShortnerEntry createdItem = await _container.CreateItemAsync(
            item: entry,
            partitionKey: new PartitionKey(entry.Id)
        );

        return createdItem.Id;
    }

    public async Task<ShortnerEntry?> GetEntry(string id)
    {
        try
        {
            return await _container.ReadItemAsync<ShortnerEntry>(
                id: id,
                partitionKey: new PartitionKey(id));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}