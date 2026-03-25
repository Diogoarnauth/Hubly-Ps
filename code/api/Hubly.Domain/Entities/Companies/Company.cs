using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("companies", Schema = "dbo")]
public class Company
{
    [Key]
    [Column("user_id")]
    public int Id { get; set; }

    [Column("company_name")]
    public string CompanyName { get; set; } = string.Empty;

    [Column("is_verified")]
    public bool IsVerified { get; set; } = false;
    [Column("description")]
    public string? Description { get; set; }
  
    [Column("sector_id")]
    public int SectorId { get; set; }

    [Column("sub_sector_id")]
    public int? SubSectorId { get; set; }

    [Column("company_size")]
    public string? CompanySize { get; set; }

    [Column("website_link")]
    public string? WebsiteLink { get; set; }

    [Column("country_headquarters")]
    public string? CountryHeadquarters { get; set; }

    public virtual Sector Sector { get; set; } = null!;
    public virtual SubSector? SubSector { get; set; }
    public virtual User User { get; set; } = null!;
}