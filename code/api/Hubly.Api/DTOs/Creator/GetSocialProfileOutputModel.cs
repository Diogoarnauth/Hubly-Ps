// Hubly.api.Models.Creators.GetSocialProfileOutputModel.cs
using Hubly.api.Domain.Entities;

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
    public string? PlatformName { get; set; }
    public List<string> Sectors { get; set; } = new();
    public bool IsOwner { get; set; } 
}