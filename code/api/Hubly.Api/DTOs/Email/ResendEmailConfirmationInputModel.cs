using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs
{
    public class ResendEmailConfirmationInputModel
    {
        [Required(ErrorMessage = "The email address is required.")]
        [EmailAddress(ErrorMessage = "The provided email is invalid")]
        [StringLength(50, ErrorMessage = "The provided email is invalid")]
        public string Email { get; set; }
    }
}
