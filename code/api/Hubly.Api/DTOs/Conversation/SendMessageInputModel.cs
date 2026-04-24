using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class SendMessageInputModel
{
    [Required]
    public string Content { get; set; } = null!;

}