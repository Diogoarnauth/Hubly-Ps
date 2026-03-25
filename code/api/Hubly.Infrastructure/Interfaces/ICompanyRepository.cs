using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ICompanyRepository
    {
        Task<bool> RegisterCompany(Company newCompany);
        Task<bool> ExistsByUserId(int userId);
        Task<Company?> GetByUserId(int userId);
        Task<Company?> EditProfile(int user_id, string company_size, string company_name, string description, int sectorId, int? subSectorId, string website_link, string country_headquarters);
        //Task<PagedResponse<Company>> Search( string Name, string sector, string CompanySize, string CountryHeadquarters , int Page, int PageSize);
        Task<int?> GetSectorIdByName(string sectorName);
        Task<int?> GetSubSectorIdByName(int sectorId, string subSectorName);


        // ...
    }
}