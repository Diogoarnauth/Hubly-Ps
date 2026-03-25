namespace Hubly.api.DTOs;

public class GetCompanyOutputModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string? Description { get; set; }
    public string? Sector { get; set; }
    public string? CompanySize { get; set; }
    public string? WebsiteLink { get; set; }
    public string? CountryHeadquarters { get; set; }
}