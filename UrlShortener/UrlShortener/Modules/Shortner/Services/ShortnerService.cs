using UrlShortener.Modules.Shortner.Models;

namespace UrlShortener.Modules.Shortner.Services;

public class ShortnerService
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
        string key = _keyGenerator.CreateKey(_settings.KeyLength);
        ShortnerEntry entry = new(key, url);

        await _repository.CreateEntry(entry);
        
        return key;
    }
}