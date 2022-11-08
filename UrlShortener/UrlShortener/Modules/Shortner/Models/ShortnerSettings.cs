using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Modules.Shortner.Models;

public class ShortnerSettings
{
    [Required]
    [Range(5, 50)]
    public int KeyLength { get; set; }

    [Required]
    public string DatabaseEndpoint { get; set; }

    [Required]
    public string PrimaryKey { get; set; }

    [Required]
    public string DatabaseName { get; set; }

    [Required]
    public string ContainerName { get; set; }
}

