using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class StatusChangeInputModel
{
    public string AvailabilityStatus { get; set; } = string.Empty;
}