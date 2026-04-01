using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;

namespace Hubly.api.DTOs;

public class CompanyOutputModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;
    public string? Description { get; set; }
    public List<string> Sectors { get; set; } = new();
    public string? CompanySize { get; set; }
    public string? WebsiteLink { get; set; }
    public string? CountryHeadquarters { get; set; }

    
}