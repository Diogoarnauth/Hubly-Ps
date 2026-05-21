using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class CompanyInputModel
{
    [Required(ErrorMessage = "Company size is required.")]
    public int CompanySize { get; set; } = 0;

    [Required(ErrorMessage = "Company name is required.")]
    public string CompanyName { get; set; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Sectors are required.")]
    public List<string> Sectors { get; set; } = new(); 
    
    [Required(ErrorMessage = "Website link is required.")]
    public string WebsiteLink { get; set; } = null!;

    [Required(ErrorMessage = "Country of headquarters is required.")]
    public string CountryHeadquarters { get; set; } = null!;

}
