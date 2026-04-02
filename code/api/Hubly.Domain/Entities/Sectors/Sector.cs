using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("sectors", Schema = "dbo")]
public class Sector
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("sector_name")]
    public string SectorName { get; set; } = string.Empty;

}