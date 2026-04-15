namespace Hubly.api.DTOs;
public class FullCompanyProfileOutputModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public CompanyOutputModel? Company { get; set; }
}
