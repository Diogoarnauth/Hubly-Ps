using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface ICompanyService
    {
        Task<OneOf<Company, CompanyError>> Register(int userId, int company_size, string company_name, string description, List<string> sectors, string website_link, string country_headquarters);
        Task<OneOf<Company?, CompanyError>> EditProfile(int user_id, int? coWorkerId, int? company_size, string company_name, string description, List<string> sectors, string website_link, string country_headquarters);
        Task<OneOf<Company, CompanyError>> GetById(int targetCompanyId, int viewerId);  
        Task<OneOf<PagedResponse<Company>, CompanyError>> Search(string? Name, List<string>? sectors, string? CompanySize, List<string>? countries, int Page, int PageSize);
        Task<OneOf<List<Company>, CompanyError>> GetTrendingCompanies(int limit);
        Task<List<string>> GetAllCountries();
        Task<OneOf<List<Company>, CompanyError>> GetRecommendedCompanies(int userId);
    }

//pesquisa por filtros

}   