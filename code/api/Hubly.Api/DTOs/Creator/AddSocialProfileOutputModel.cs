using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class AddSocialProfileOutputModel
{
    public int Id {get; set;}
    public string PlatformUserName { get; set; } = null!;  
    public string Link { get; set; } = null!;
    public int FollowersCount { get; set; }   
    public decimal? PriceMin { get; set; } = null;
    public decimal? PriceMax { get; set; } = null;
    public int PlatformId { get; set; } 
}