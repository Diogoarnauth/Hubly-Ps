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
    public string? Description { get; set; }
    public string? Sector { get; set; }

    [Column("company_size")]
    public int? CompanySize { get; set; }

    [Column("website_link")]
    public string? WebsiteLink { get; set; }

    [Column("country_headquarters")]
    public string? CountryHeadquarters { get; set; }

    public virtual User User { get; set; } = null!;
}