using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class CompanyCreateInputModel
{
    [Required]
    public int CompanySize { get; set; } = 0;

    [Required]
    public string CompanyName { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Sector { get; set; } = null!;

    public string? SubSector { get; set; } = null!;

    [Required]
    public string WebsiteLink { get; set; } = null!;

    [Required]
    public string CountryHeadquarters { get; set; } = null!;

}
