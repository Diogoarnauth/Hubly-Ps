using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class TagConversationInputModel
{
    [Required]
    public int TagId { get; set; }
}
