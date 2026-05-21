using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class EditUserInputModel
{
    [Required(ErrorMessage = "The new username is required.")]
    [StringLength(25, MinimumLength = 3, ErrorMessage = "The username provided is invalid")]
    public string NewUsername { get; set; }
}