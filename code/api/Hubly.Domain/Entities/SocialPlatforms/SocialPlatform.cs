using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("social_platforms", Schema = "dbo")]
public class SocialPlatform
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name_platform")] 
    public string NamePlatform { get; set; } = string.Empty;

    public virtual ICollection<CreatorSocialProfile> CreatorProfiles { get; set; } = new List<CreatorSocialProfile>();
}