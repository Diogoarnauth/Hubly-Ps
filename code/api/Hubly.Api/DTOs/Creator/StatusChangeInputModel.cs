using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class StatusChangeInputModel
{
    [Required]
    public string AvailabilityStatus { get; set; } = string.Empty;
}