using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Modules.Shortner.Models;

public record CreateShortenedUrlRequest([Required]string Url);