using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;
using System.Data.Common;
using System.Linq;
using Microsoft.Extensions.Configuration;


namespace Hubly.api.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly CompaniesDomain _companiesDomain;

        public CompanyService(
            ITransactionManager transactionManager,
            IConfiguration configuration,
            CompaniesDomain companiesDomain
        )
        {
            _transactionManager = transactionManager;
            _companiesDomain = companiesDomain;

        }

        public async Task<OneOf<Company, CompanyError>> Register(int userId, int company_size, string company_name, string description, List<string> sectors, string website_link, string country_headquarters)
        {
            if (!_companiesDomain.IsValidWebsite(website_link)) return new CompanyError.InvalidWebSiteLink();
            if (!_companiesDomain.IsValidCountry(country_headquarters)) return new CompanyError.InvalidCountryHeadquarters();

            string sizeCategory = _companiesDomain.ConvertCompanySize(company_size);

            var result = await _transactionManager.Run<OneOf<Company, CompanyError>>(async (context) =>
            {
                if (await context.CompanyRepository.ExistsByCompanyName(company_name))
                    return new CompanyError.CompanyAlreadyExists(); 

                var foundSectors = await context.CompanyRepository.GetSectorByName(sectors);

                if (foundSectors.Count != sectors.Count) return new CompanyError.InvalidSectorName();

                if (await context.CompanyRepository.ExistsByUserId(userId))
                    return new CompanyError.CompanyAlreadyExists();

                if (await context.CreatorRepository.ExistsByUserId(userId))
                    return new CompanyError.UserAlreadyRegisteredAsCreator();

                var newCompany = new Company
                {
                    Id = userId,
                    CompanyName = company_name,
                    IsVerified = false,
                    Description = description,
                    Sectors = foundSectors,
                    CompanySize = sizeCategory,
                    WebsiteLink = website_link,
                    CountryHeadquarters = country_headquarters
                };

                await context.CompanyRepository.RegisterCompany(newCompany);

                return newCompany;
            });

            return result;
        }

        public async Task<OneOf<Company, CompanyError>> EditProfile(int user_id, int company_size, string company_name, string description, List<string> sectors, string website_link, string country_headquarters)
        {
            if (!_companiesDomain.IsValidWebsite(website_link)) return new CompanyError.InvalidWebSiteLink();
            if (!_companiesDomain.IsValidCountry(country_headquarters)) return new CompanyError.InvalidCountryHeadquarters();

            string sizeCategory = _companiesDomain.ConvertCompanySize(company_size);

            var result = await _transactionManager.Run<OneOf<Company, CompanyError>>(async (context) =>
            {
                if (await context.CreatorRepository.ExistsByUserId(user_id)) return new CompanyError.UserAlreadyRegisteredAsCreator();

                var companyExists = await context.CompanyRepository.GetByUserId(user_id);
                if (companyExists == null) return new CompanyError.FailedToGetCompanyInfo();

                var foundSectors = await context.CompanyRepository.GetSectorByName(sectors);
                if (foundSectors.Count != sectors.Count) return new CompanyError.InvalidSectorName();

                var updatedCompany = await context.CompanyRepository.EditProfile(
                    user_id, sizeCategory, company_name, description, foundSectors, website_link, country_headquarters
                );

                if (updatedCompany == null) return new CompanyError.FailedToGetCompanyInfo();

                return updatedCompany;
            });

            return result;
        }

        public async Task<OneOf<Company, CompanyError>> GetById(int targetCompanyId, int viewerId)
        {
            var result = await _transactionManager.Run<OneOf<Company, CompanyError>>(async (context) =>
            {
                var company = await context.CompanyRepository.GetByUserId(targetCompanyId);
                if (company == null) return new CompanyError.CompanyNotFound();
                var isOwner = targetCompanyId == viewerId;
                Console.WriteLine($"Company viewed: {company.CompanyName} (ID: {company.Id}) by User ID: {viewerId} - Is Owner: {isOwner}");

                if (!isOwner){
                try
                {
                    var historyEntry = new ProfileViewHistory
                    {
                        ViewerUserId = viewerId,
                        ViewedCompanyId = targetCompanyId,
                        ViewedAt = DateTime.UtcNow
                    };

                    await context.HistoryRepository.AddView(historyEntry);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao gravar histórico: {ex.Message}");
                }
                }

                return company;
            });
        

            return result;
        }


        public async Task<OneOf<PagedResponse<Company>, CompanyError>> Search(string? Name, List<string>? sectors, string? CompanySize, List<string>? countries, int Page, int PageSize)
        {
            Page = Page <= 0 ? 1 : Page;
            PageSize = PageSize <= 0 ? 10 : (PageSize > 100 ? 100 : PageSize);

            return await _transactionManager.Run<OneOf<PagedResponse<Company>, CompanyError>>(async (context) =>
            {
                var results = await context.CompanyRepository.Search(
                    Name,
                    sectors,
                    CompanySize,
                    countries,
                    Page,
                    PageSize
                );

                if (results == null) return new CompanyError.FailedToGetCompanyInfo();

                return results;
            });
        }

        public async Task<List<string>> GetAllCountries()
        {
            return await Task.Run(() => _companiesDomain.GetSupportedCountries());
        }


        public async Task<OneOf<List<Company>, CompanyError>> GetTrendingCompanies(int limit)
        {
            return await _transactionManager.Run<OneOf<List<Company>, CompanyError>>(async (context) =>
            {
                var companies = await context.HistoryRepository.GetTopTrendingCompanies(limit);

                if (companies == null)
                    return new List<Company>();

                return companies;
            });
        }

        public async Task<OneOf<List<Company>, CompanyError>> GetRecommendedCompanies(int userId)
        {
            return await _transactionManager.Run<OneOf<List<Company>, CompanyError>>(async (context) =>
            {
                var userInterests = await context.HistoryRepository.GetUserInterests(userId, 50);

                bool hasHistory = userInterests.SectorFrequencies.Any() ||
                                  userInterests.CountryFrequencies.Any() ||
                                  userInterests.SizeFrequencies.Any();

                if (!hasHistory)
                {
                    return new List<Company>();
                }

                var recommendations = await context.CompanyRepository.GetRecommendedByScore(
                    userId,
                    userInterests
                );

                return recommendations ?? new List<Company>();
            });
        }
    }
}