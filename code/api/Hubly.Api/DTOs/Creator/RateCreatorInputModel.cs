using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class RateCreatorInputModel
{
    [Required]
    public int Rate { get; set; } 
}