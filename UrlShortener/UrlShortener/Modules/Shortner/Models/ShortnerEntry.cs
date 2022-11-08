namespace UrlShortener.Modules.Shortner.Models;

public record struct ShortnerEntry(string Key, string Url)
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
}