using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class EditPlatformsInputModel
{
    [Required (ErrorMessage = "The platform name is required.")]
    public string PlatformUserName { get; set; } = null!;

    [Required (ErrorMessage = "The platform link is required.")]
    public string Link { get; set; } = null!;
    
    [Required (ErrorMessage = "The platform description is required.")]
    public string Description { get; set; } = null!;

    [Required (ErrorMessage = "The follower count is required.")]
    public int FollowersCount { get; set; }

    public decimal? PriceMin { get; set; } = null;

    public decimal? PriceMax { get; set; } = null;
    public List<string> Sectors { get; set; } = new(); 
  
}