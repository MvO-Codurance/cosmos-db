namespace UrlShortener.Modules.Shortner.Services;

public interface IShortnerService
{
    Task<string> CreateShortenedUrl(string url);
    Task<string?> GetOriginalUrl(string id);
}