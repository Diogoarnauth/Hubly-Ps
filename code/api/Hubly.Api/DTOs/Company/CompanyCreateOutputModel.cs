using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;

namespace Hubly.api.DTOs;

public class CompanyCreateOutputModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;
    public string? Description { get; set; }
    public string? Sector { get; set; }
    public string? CompanySize { get; set; }
    public string? WebsiteLink { get; set; }
    public string? CountryHeadquarters { get; set; }

    
}