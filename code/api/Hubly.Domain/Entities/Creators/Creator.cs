using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("creators", Schema = "dbo")]
public class Creator
{
    [Key]
    [Column("user_id")] 
    public int Id { get; set; }

    [Required]
    [Column("artistic_name")]
    public string ArtisticName { get; set; } = null!;

    [Column("is_verified")]
    public bool IsVerified { get; set; } = false;

    [Required]
    [Column("availability_status")]
    public string AvailabilityStatus { get; set; } = "AVAILABLE";

    [Column("global_rating")]
    public decimal GlobalRating { get; set; } = 0;

    [Column("ratings_count")]
    public int RatingsCount { get; set; } = 0;

    [Column("chats_started_count")]
    public int ChatsStartedCount { get; set; } = 0;

    [Column("chats_responded_count")]
    public int ChatsRespondedCount { get; set; } = 0;

    [ForeignKey("Id")]
    public virtual User User { get; set; } = null!;

    //public virtual ICollection<CreatorSocialProfile> SocialProfiles { get; set; } = new List<CreatorSocialProfile>();
}