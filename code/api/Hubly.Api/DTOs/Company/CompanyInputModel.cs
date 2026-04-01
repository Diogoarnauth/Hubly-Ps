using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class CompanyInputModel
{
    [Required]
    public int CompanySize { get; set; } = 0;

    [Required]
    public string CompanyName { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public List<string> Sectors { get; set; } = new(); 
    
    [Required]
    public string WebsiteLink { get; set; } = null!;

    [Required]
    public string CountryHeadquarters { get; set; } = null!;

}
