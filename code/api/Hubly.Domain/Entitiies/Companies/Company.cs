using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.Domain.Entities;

[Table("companies", Schema = "dbo")]
public class Company : User
{
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
}