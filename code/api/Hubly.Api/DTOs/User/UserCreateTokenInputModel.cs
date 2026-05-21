using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class UserCreateTokenInputModel
{
    [Required(ErrorMessage = "Invalid Credentials (email or password)")]
    [EmailAddress(ErrorMessage = "Invalid Credentials (email or password)")]
    [StringLength(50, ErrorMessage = "Invalid Credentials (email or password)")]
    public string Email { get; set; } = null!;
        
    [Required(ErrorMessage = "Invalid Credentials (email or password)")]
    [StringLength(25, ErrorMessage = "Invalid Credentials (email or password)")]
    public string Password { get; set; } = null!;
}