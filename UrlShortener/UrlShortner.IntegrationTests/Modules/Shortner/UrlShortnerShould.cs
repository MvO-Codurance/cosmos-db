using System.Net;
using AutoFixture.Xunit2;
using FluentAssertions;
using UrlShortner.IntegrationTests.Modules.Shortner.Models;
using Xunit;
using Xunit.Abstractions;

namespace UrlShortner.IntegrationTests.Modules.Shortner;

[Collection(CollectionFixture.CollectionDefinitionName)]
public class UrlShortnerShould : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;

    public UrlShortnerShould(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _factory.OutputHelper = outputHelper;
        _factory.BaseUri = "shortner";
    }
    
    public void Dispose() => _factory.OutputHelper = null;
    
    [Theory]
    [InlineAutoData]
    public async Task Create_A_Shortened_Url_Entry(string url)
    {
        //arrange
        HttpClient client = _factory.CreateClient();
        var request = new
        {
            Url = url
        };
        
        // act
        using HttpResponseMessage response = await client.PostAsJsonAsync(_factory.BuildUri(), request);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBody = await response.Content.ReadFromJsonAsync<CreateEntryResponseBody>();
        responseBody.Should().NotBeNull();
        responseBody!.Key.Should().NotBeNullOrEmpty();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task Return_The_Correct_Url_For_A_Previously_Created_Entry(string url)
    {
        //arrange
        HttpClient client = _factory.CreateClient();
        var createEntryRequest = new
        {
            Url = url
        };
        
        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(_factory.BuildUri(), createEntryRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createResponseBody = await createResponse.Content.ReadFromJsonAsync<CreateEntryResponseBody>();
        createResponseBody.Should().NotBeNull();
        createResponseBody!.Key.Should().NotBeNullOrEmpty();
        
        // act
        using HttpResponseMessage getResponse = await client.GetAsync(_factory.BuildUri(createResponseBody.Key));

        // assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseBody = await getResponse.Content.ReadFromJsonAsync<GetEntryResponseBody>();
        getResponseBody.Should().NotBeNull();
        getResponseBody!.Url.Should().Be(url);
    }
    
    [Theory]
    [InlineAutoData]
    public async Task Return_Null_For_An_Unknown_Entry(string key)
    {
        //arrange
        HttpClient client = _factory.CreateClient();
        
        // act
        using HttpResponseMessage response = await client.GetAsync(_factory.BuildUri(key));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}