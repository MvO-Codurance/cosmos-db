namespace UrlShortener.Modules.Shortner.Models;

public record ShortnerEntry(string Key, string Url)
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Key { get; set; } = Key;

    public string Url { get; set; } = Url;
}