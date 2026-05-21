using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class CreatorCreateInputModel
{
    [Required (ErrorMessage = "The artistic name is required.")]
    public string ArtisticName { get; set; } = null!;

}