using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class AddSocialProfileInputModel
{
    [Required]
    public string Platform_user_name { get; set; } = null!;

    [Required]
    public string Link { get; set; } = null!;

    [Required]
    public int Followers_count { get; set; }

    public decimal? PriceMin { get; set; } = null;

    public decimal? PriceMax { get; set; } = null;

    [Required]
    public int PlatformId { get; set; } 
}