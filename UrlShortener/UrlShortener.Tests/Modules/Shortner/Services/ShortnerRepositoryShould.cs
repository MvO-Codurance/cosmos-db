using System.Reflection;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using UrlShortener.Modules.Shortner.Models;
using UrlShortener.Modules.Shortner.Services;
using Xunit;

namespace UrlShortener.Tests.Modules.Shortner.Services;

public class ShortnerRepositoryShould
{
    [Theory]
    [InlineAutoNSubstituteData]
    public async Task Create_A_New_Shortner_Entry(
        ShortnerEntry entry)
    {
        var settings = GetSettingsForEmulator();
        var cosmosClient = CreateCosmosClient(settings);
        var sut = new ShortnerRepository(settings, cosmosClient);
        
        var actual = await sut.CreateEntry(entry);

        actual.Should().NotBeNull();
        actual.Should().Be(entry.Id);
    }

    private static ShortnerSettings GetSettingsForEmulator()
    {
        var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(fileProvider, "appsettings.json", false, false)
            .Build();

        var settings = new ShortnerSettings();
        configuration.Bind("UrlShortner", settings);

        return settings;
    }
    
    private static CosmosClient CreateCosmosClient(ShortnerSettings settings)
    {
        var cosmosClientOptions = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };
        
        return new CosmosClient(
            settings.DatabaseEndpoint, 
            settings.PrimaryKey, 
            cosmosClientOptions);
    }
}