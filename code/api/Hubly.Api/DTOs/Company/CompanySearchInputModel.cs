namespace Hubly.api.DTOs;

public class CompanySearchInputModel 
{
    public string? Name { get; set; }
    public string? Sector { get; set; }
    public List<string>? SubSector {get; set;} 
    public string? CompanySize { get; set; }
    public List<string>? Countries { get; set; } 
    public int Page { get; set; } = 1;
    public int PageSize {get; set; } = 10;
}