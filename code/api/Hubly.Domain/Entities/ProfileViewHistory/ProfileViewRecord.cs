using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("profile_views_history", Schema = "dbo")]
public class ProfileViewHistory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("viewer_user_id")]
    public int ViewerUserId { get; set; }

    [Column("viewed_company_id")]
    public int? ViewedCompanyId { get; set; }

    [Column("viewed_social_profile_id")]
    public int? ViewedSocialProfileId { get; set; }

    [Column("viewed_at")]
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("ViewerUserId")]
    public virtual User Viewer { get; set; } = null!;

    [ForeignKey("ViewedCompanyId")]
    public virtual Company? ViewedCompany { get; set; }

    [ForeignKey("ViewedSocialProfileId")] 
    public virtual CreatorSocialProfile? ViewedSocialProfile { get; set; }
}