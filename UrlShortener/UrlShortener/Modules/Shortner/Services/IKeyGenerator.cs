namespace UrlShortener.Modules.Shortner.Services;

public interface IKeyGenerator
{
    string CreateKey(int length);
}