// Hubly.api.Models.Creators.GetSocialProfileOutputModel.cs
namespace Hubly.api.DTOs;

public class GetSocialProfileOutputModel
{
    public int Id { get; set; } 
    public int CreatorId { get; set; }
    public int PlatformId { get; set; }
    public string PlatformUserName { get; set; }
    public string Link { get; set; }
    public string? Description { get; set; }
    public int FollowersCount { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
}