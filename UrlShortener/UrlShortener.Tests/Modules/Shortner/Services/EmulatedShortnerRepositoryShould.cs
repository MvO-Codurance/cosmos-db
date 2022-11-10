using System.Reflection;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using UrlShortener.Modules.Shortner.Models;
using UrlShortener.Modules.Shortner.Services;
// ReSharper disable once RedundantUsingDirective
using Xunit;
// comment out next line to enable these tests (they hit a Cosmos instance so they are disabled by default)
using TheoryAttribute = UrlShortener.Tests.DisableTestAttribute;

#pragma warning disable xUnit1008 // Test data attribute should only be used on a Theory. Remove the test data, or add the Theory attribute to the test method.

namespace UrlShortener.Tests.Modules.Shortner.Services;

public class EmulatedShortnerRepositoryShould
{
    [Theory]
    [InlineAutoMoqData]
    public async Task Create_A_New_Shortner_Entry(
        ShortnerEntry entry)
    {
        var sut = GetShortnerRepository();
        
        var actualId = await sut.CreateEntry(entry);

        actualId.Should().NotBeNull();
        actualId.Should().Be(entry.Id);
    }
    
    [Theory]
    [InlineAutoMoqData]
    public async Task Retrieve_A_Previously_Created_Entry_Using_The_Id(
        ShortnerEntry entry)
    {
        var sut = GetShortnerRepository();
        
        var storedEntryId = await sut.CreateEntry(entry);
        storedEntryId.Should().NotBeNull();

        var actualEntry = await sut.GetEntry(entry.Id);
        actualEntry.Should().BeEquivalentTo(entry);
    }
    
    [Theory]
    [InlineAutoMoqData]
    public async Task Retrieve_Null_For_An_Unknown_Id(
        ShortnerEntry entry)
    {
        var sut = GetShortnerRepository();

        var actualEntry = await sut.GetEntry(entry.Id);
        actualEntry.Should().BeNull();
    }

    private static ShortnerRepository GetShortnerRepository()
    {
        var settings = GetSettingsForEmulator();
        var cosmosClient = CreateCosmosClient(settings);
        return new ShortnerRepository(settings, cosmosClient);
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