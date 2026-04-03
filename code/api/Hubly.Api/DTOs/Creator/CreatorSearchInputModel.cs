namespace Hubly.api.DTOs;
public class CreatorSearchInputModel 
{
    public int? PlatformId { get; set; }
    public string? PlatformUserName { get; set; }
    public int? FollowersCountMin { get; set; }
    public int? FollowersCountMax { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public List<string>? Sectors { get; set; } 
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}