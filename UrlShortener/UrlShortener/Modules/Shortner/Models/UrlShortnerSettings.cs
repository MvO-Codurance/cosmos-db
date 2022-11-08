using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Modules.Shortner.Models;

public class UrlShortnerSettings
{
    [Required]
    [Range(5, 50)]
    public int KeyLength { get; set; }
}