using Hubly.api.Domain.Entities;

namespace Hubly.api.DTOs;

public class GetCreatorOutputModel
{
    public int Id { get; set; } 
    public string ArtisticName { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string AvailabilityStatus { get; set; } = string.Empty;
    public decimal? GlobalRating { get; set; }
    public int RatingsCount { get; set; }
    public int ChatsStartedCount { get; set; } 
    public int ChatsRespondedCount { get; set; } 
    public List<SocialProfileOutputModel> SocialProfiles { get; set; } = new();
}