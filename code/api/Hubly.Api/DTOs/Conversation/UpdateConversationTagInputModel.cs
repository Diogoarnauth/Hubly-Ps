using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class UpdateConversationTagInputModel
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string TagName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$")]
    public string ColorHex { get; set; } = "#808080";
}
