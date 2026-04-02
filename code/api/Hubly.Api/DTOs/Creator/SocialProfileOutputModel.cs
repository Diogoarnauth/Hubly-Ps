namespace Hubly.api.DTOs;

public class SocialProfileOutputModel
{
    public int Id { get; set; }
    public int PlatformId { get; set; }
    public string? PlatformUserName { get; set; }
    public string? Link { get; set; }
    public string? Description { get; set; }
    public int FollowersCount { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public string? PlatformName { get; set; } 
}

