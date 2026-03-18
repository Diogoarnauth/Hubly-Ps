using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs
{
    public class ResendEmailConfirmationInputModel
    {
        [Required]
        public string Email { get; set; }
    }
}
