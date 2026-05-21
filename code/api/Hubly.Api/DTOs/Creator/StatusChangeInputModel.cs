using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class StatusChangeInputModel
{
    [Required (ErrorMessage = "The availability status is required.")]
    public string AvailabilityStatus { get; set; } = string.Empty;
}