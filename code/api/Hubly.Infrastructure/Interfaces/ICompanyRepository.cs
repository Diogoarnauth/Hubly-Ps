using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ICompanyRepository
    {
        Task<bool> RegisterCompany(Company newCompany);
        Task<bool> ExistsByUserId(int userId);
        Task<Company?> GetByUserId(int userId);
        Task<bool> ExistsByCompanyName(string companyName);
        Task<Company?> EditProfile(int user_id, string company_size, string company_name, string description, List<Sector> sectors, string website_link, string country_headquarters);
        Task<PagedResponse<Company>> Search(string? Name, List<string>? sectors, string? CompanySize, List<string>? countries, int page, int pageSize);
        Task<List<Sector>> GetSectorByName(List<string> sectorName);
        Task<List<Company>> GetRecommendedByScore(int userId, UserInterestProfile profile);

        // ...
    }
}