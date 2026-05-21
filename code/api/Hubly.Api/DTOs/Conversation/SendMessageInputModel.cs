using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class SendMessageInputModel
{
    [Required(ErrorMessage = "Message content is required.")]
    public string Content { get; set; } = null!;

}