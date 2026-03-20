using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;

namespace Hubly.api.DTOs;

public class CompanyEditOutputModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Sector { get; set; }
    public int? CompanySize { get; set; }
    public string? WebsiteLink { get; set; }
    public string? CountryHeadquarters { get; set; }

    
}