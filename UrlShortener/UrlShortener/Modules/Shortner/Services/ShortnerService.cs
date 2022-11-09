using UrlShortener.Modules.Shortner.Models;

namespace UrlShortener.Modules.Shortner.Services;

public class ShortnerService : IShortnerService
{
    private readonly ShortnerSettings _settings;
    private readonly IKeyGenerator _keyGenerator;
    private readonly IShortnerRepository _repository;

    public ShortnerService(
        ShortnerSettings settings,
        IKeyGenerator keyGenerator,
        IShortnerRepository repository)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _keyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task<string> CreateShortenedUrl(string url)
    {
        string id = _keyGenerator.CreateKey(_settings.KeyLength);
        ShortnerEntry entry = new(id, url);

        await _repository.CreateEntry(entry);
        
        return id;
    }

    public async Task<string?> GetOriginalUrl(string id)
    {
        var entry = await _repository.GetEntry(id);
        return entry?.Url;
    }
}