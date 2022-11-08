using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using UrlShortener.Modules.Shortner.Models;
using UrlShortener.Modules.Shortner.Services;

namespace UrlShortener.Modules.Shortner;

public class ShortnerModule: IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<ShortnerSettings>(configuration.GetSection("UrlShortner"));
        services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<ShortnerSettings>>().Value);
        services.AddSingleton<IKeyGenerator, KeyGenerator>();
        
        services.AddSingleton<IShortnerRepository>((serviceProvider) =>
        {
            var settings = serviceProvider.GetRequiredService<ShortnerSettings>();
            var cosmosClientOptions = new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };
            var cosmosClient = new CosmosClient(settings.DatabaseEndpoint, settings.PrimaryKey, cosmosClientOptions);
            return new ShortnerRepository(settings, cosmosClient);
        });
        
        services.AddSingleton<IShortnerService, ShortnerService>();
        
        return services;
    }
    
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/shortner", async (
            [Required][FromBody] CreateShortenedUrlRequest request,
            IShortnerService shortnerService) =>
        {
            var key = await shortnerService.CreateShortenedUrl(request.Url);
            return new CreateShortenedUrlResponse(key);
        });
        
        return endpoints;
    }
}