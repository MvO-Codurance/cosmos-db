using System.Security.Cryptography;

namespace UrlShortener.Modules.Shortner.Services;

public class KeyGenerator : IKeyGenerator
{
    public string CreateKey(int length)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(length));
    }
}