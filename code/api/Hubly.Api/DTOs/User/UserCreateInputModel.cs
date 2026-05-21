using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class UserCreateInputModel
{
    [Required(ErrorMessage = "The provided name is invalid")]
    [StringLength(25, MinimumLength = 3, ErrorMessage = "The provided name is invalid")]
    [RegularExpression(@"^[a-zA-Z0-9\s._'-]+$", ErrorMessage = "The provided name is invalid")]
    public string Name { get; set; } = null!;        

    [Required(ErrorMessage = "The provided email is invalid")]
    [EmailAddress(ErrorMessage = "The provided email is invalid")]
    [StringLength(50, ErrorMessage = "The provided email is invalid")]
    public string Email { get; set; } = null!;
        
    [Required(ErrorMessage = "The password provided is invalid")]
    public string Password { get; set; } = null!;
}