using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ICompanyRepository
    {
        Task<bool> RegisterCompany(Company newCompany);
        Task<bool> ExistsByUserId(int userId);
        Task<Company?> GetByUserId(int userId);
        Task<Company?> EditProfile(int user_id, int company_size, string company_name, string description, string sector, string website_link, string country_headquarters);
        // ...
    }
}