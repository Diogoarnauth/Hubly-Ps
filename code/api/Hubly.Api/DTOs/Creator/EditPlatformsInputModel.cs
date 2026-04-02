using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class EditPlatformsInputModel
{
    [Required]
    public string PlatformUserName { get; set; } = null!;

    [Required]
    public string Link { get; set; } = null!;
    
    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public int FollowersCount { get; set; }

    public decimal? PriceMin { get; set; } = null;

    public decimal? PriceMax { get; set; } = null;
    public List<string> Sectors { get; set; } = new(); 
  
}