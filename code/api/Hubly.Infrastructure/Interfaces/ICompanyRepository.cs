using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ICompanyRepository
    {
        Task<bool> RegisterCompany(Company newCompany);
        Task<bool> ExistsByUserId(int userId);
        Task<Company?> GetByUserId(int userId);
        //Task EditProfile(int userId, string companyName, string sector, string description);
        // ...
    }
}