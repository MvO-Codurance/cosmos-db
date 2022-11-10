using System.Net;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace UrlShortner.IntegrationTests;

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
    public async Task Create_A_New_Entry_Via_Http_Post(string url)
    {
        HttpClient client = _factory.CreateClient();

        var request = new
        {
            Url = url
        };
        
        // act
        using HttpResponseMessage response = await client.PostAsJsonAsync(_factory.BuildUri(), request);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}