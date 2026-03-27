using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("creator_social_profiles", Schema = "dbo")]
public class CreatorSocialProfile
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("creator_id")]
    public int CreatorId { get; set; }

    [Required]
    [Column("platform_id")]
    public int PlatformId { get; set; }

    [Column("platform_user_name")]
    [MaxLength(100)]
    public string? PlatformUserName { get; set; }

    [Column("link")]
    [MaxLength(255)]
    public string? Link { get; set; }

    [Column("followers_count")]
    public int FollowersCount { get; set; } = 0;

    [Column("price_min")]
    public decimal? PriceMin { get; set; }

    [Column("price_max")]
    public decimal? PriceMax { get; set; }

    // Navigation Properties
    [ForeignKey("CreatorId")]
    public virtual Creator Creator { get; set; } = null!;

    [ForeignKey("PlatformId")]
    public virtual SocialPlatform Platform { get; set; } = null!;
}