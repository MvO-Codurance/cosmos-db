using UrlShortener.Modules.Shortner.Models;

namespace UrlShortener.Modules.Shortner.Services;

public interface IShortnerRepository
{
    Task<string> CreateEntry(ShortnerEntry entry);
    Task<ShortnerEntry?> GetEntry(string id);
}