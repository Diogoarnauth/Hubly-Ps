using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs
{
    public class VerifyEmailInputModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
