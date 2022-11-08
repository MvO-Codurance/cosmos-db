using UrlShortener.Modules.Weather.Services;

namespace UrlShortener.Modules.Weather;

public class WeatherModule: IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        services.AddSingleton<IWeatherService, WeatherService>();
        return services;
    }
    
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/weather-forecast", (IWeatherService weatherService) => weatherService.GetForecast());
        
        return endpoints;
    }
}