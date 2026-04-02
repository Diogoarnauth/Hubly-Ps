using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class CreatorCreateInputModel
{
    [Required]
    public string ArtisticName { get; set; } = null!;

}