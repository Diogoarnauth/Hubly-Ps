using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.Domain.Entities;

[Table("creators", Schema = "dbo")]
public class Creator : User
{
    [Column("artistic_name")]
    public string? ArtisticName { get; set; }
    public string? Content { get; set; }
    public string? Audience { get; set; }
}