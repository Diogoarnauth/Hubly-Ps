using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("creators", Schema = "dbo")]
public class Creator
{
    [Key]
    [Column("user_id")] 
    public int Id { get; set; }

    [Column("artistic_name")]
    public string? ArtisticName { get; set; }
    public string? Content { get; set; }
    public string? Audience { get; set; }

  
    public virtual User User { get; set; } = null!;
}