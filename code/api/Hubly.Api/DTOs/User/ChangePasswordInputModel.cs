using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class ChangePasswordInputModel
{
    [Required(ErrorMessage = "The old password is required.")]
    public string OldPassword { get; set; } = null!;

    [Required(ErrorMessage = "The new password is required.")]
    public string NewPassword { get; set; } = null!;
}