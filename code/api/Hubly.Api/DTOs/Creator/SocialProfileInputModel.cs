using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class SocialProfileInputModel
{
    [Required (ErrorMessage = "The platform username is required.")]
    public string Platform_user_name { get; set; } = null!;

    [Required (ErrorMessage = "The platform link is required.")]
    public string Link { get; set; } = null!;
    
    [Required (ErrorMessage = "The platform description is required.")]
    public string Description { get; set; } = null!;

    [Required (ErrorMessage = "The follower count is required.")]
    public int Followers_count { get; set; }

    public decimal? PriceMin { get; set; } = null;

    public decimal? PriceMax { get; set; } = null;

    [Required (ErrorMessage = "The platform ID is required.")]
    public int PlatformId { get; set; } 
    
    [Required (ErrorMessage = "The sectors are required.")]
    public List<string> Sectors { get; set; } = new(); 

}