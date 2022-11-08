using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Modules.Shortner.Models;

public class ShortnerSettings
{
    [Required]
    [Range(5, 50)]
    public int KeyLength { get; set; }

    // CS8618: Non-nullable property is uninitialized
#pragma warning disable CS8618
    [Required]
    public string DatabaseEndpoint { get; set; }

    [Required]
    public string PrimaryKey { get; set; }

    [Required]
    public string DatabaseName { get; set; }

    [Required]
    public string ContainerName { get; set; }
#pragma warning restore CS8618
}

