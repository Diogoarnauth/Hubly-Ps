using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("sub_sectors", Schema = "dbo")]
public class SubSector
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("sector_id")] 
    public int SectorId { get; set; }

    [Column("subsector_name")]
    public string SubSectorName { get; set; } = string.Empty;

    public virtual Sector  Sector { get; set; } = null!;
}