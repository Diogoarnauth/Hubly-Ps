namespace Hubly.api.DTOs;

public class CompanySearchInputModel 
{
    public string? Name { get; set; }
    public string? Sector { get; set; }
    public string? CompanySize { get; set; }
    public string? CountryHeadquarters { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize {get; set; } = 10;
}