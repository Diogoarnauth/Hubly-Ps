using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs
{
    public class ResendEmailConfirmationRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
