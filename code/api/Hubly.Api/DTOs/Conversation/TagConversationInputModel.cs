using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class TagConversationInputModel
{
    [Required (ErrorMessage = "Conversation ID is required.")]
    public int TagId { get; set; }
}
