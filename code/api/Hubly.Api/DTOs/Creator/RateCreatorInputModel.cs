using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class RateCreatorInputModel
{
    [Required (ErrorMessage = "The creator ID is required.")]
    [Range(1, 5, ErrorMessage = "The rating score must be an integer between 1 and 5.")]
    public int Rate { get; set; } 
}