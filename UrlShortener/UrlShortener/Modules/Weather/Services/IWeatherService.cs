using UrlShortener.Modules.Weather.Models;

namespace UrlShortener.Modules.Weather.Services;

public interface IWeatherService
{
    WeatherForecast[] GetForecast();
}