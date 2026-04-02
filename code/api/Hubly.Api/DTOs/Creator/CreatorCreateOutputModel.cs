using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;

namespace Hubly.api.DTOs;

public class CreatorCreateOutputModel
{
    public int Id { get; set; } 
    public string ArtisticName { get; set; } = null!;
    public bool IsVerified { get; set; }
    public string AvailabilityStatus { get; set; } = null!;
    public decimal GlobalRating { get; set; }
    public int RatingsCount { get; set; }
    public int ChatsStartedCount { get; set; }
    public int ChatsRespondedCount { get; set; }

    
}