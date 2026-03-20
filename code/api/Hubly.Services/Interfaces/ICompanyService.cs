using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface ICompanyService
    {
        Task<OneOf<Company, CompanyError>> Register(int userId, int company_size, string company_name, string description, string sector, string website_link, string country_headquarters);
        Task<OneOf<Company?, CompanyError>> EditProfile(int user_id, int company_size, string company_name, string description, string sector, string website_link, string country_headquarters);
    }

}   