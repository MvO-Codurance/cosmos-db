using Xunit;

namespace UrlShortner.IntegrationTests;

[CollectionDefinition(CollectionDefinitionName)]
public class CollectionFixture : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string CollectionDefinitionName = "Integration Tests Collection";
}