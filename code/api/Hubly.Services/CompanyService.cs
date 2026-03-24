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

        public async Task<OneOf<Company, CompanyError>> Register(int userId, int company_size, string company_name, string description, string sector, string website_link, string country_headquarters)
        {
            if (!_companiesDomain.IsSafeText(sector)) return new CompanyError.InvalidSectorName();

            if (!_companiesDomain.IsValidWebsite(website_link)) return new CompanyError.InvalidWebSiteLink();

            if (!_companiesDomain.IsValidCountry(country_headquarters)) return new CompanyError.InvalidCountryHeadquarters();

            string sizeCategory = _companiesDomain.ConvertCompanySize(company_size);


            return await _transactionManager.Run<OneOf<Company, CompanyError>>(async (context) =>
            {
                // Verificações cruzadas
                if (await context.CompanyRepository.ExistsByUserId(userId))
                    return new CompanyError.CompanyAlreadyExists();

                if (await context.CompanyRepository.ExistsByUserId(userId))
                    return new CompanyError.UserAlreadyRegisteredAsCreator();

                var newCompany = new Company
                {
                    Id = userId,
                    CompanyName = company_name,
                    IsVerified = false,
                    Description = description,
                    Sector = sector,
                    CompanySize = sizeCategory,
                    WebsiteLink = website_link,
                    CountryHeadquarters = country_headquarters
                };

                await context.CompanyRepository.RegisterCompany(newCompany);

                return newCompany;
            });
        }
        public async Task<OneOf<Company, CompanyError>> EditProfile(int user_id, int company_size, string company_name, string description, string sector, string website_link, string country_headquarters)
        {
            if (!_companiesDomain.IsSafeText(sector)) return new CompanyError.InvalidSectorName();

            if (!_companiesDomain.IsValidWebsite(website_link)) return new CompanyError.InvalidWebSiteLink();

            if (!_companiesDomain.IsValidCountry(country_headquarters)) return new CompanyError.InvalidCountryHeadquarters();

            string sizeCategory = _companiesDomain.ConvertCompanySize(company_size);

            return await _transactionManager.Run<OneOf<Company, CompanyError>>(async (context) =>
            {
                var companyExists = await context.CompanyRepository.GetByUserId(user_id);
                if (companyExists == null) return new CompanyError.FailedToGetCompanyInfo();

                var updatedCompany = await context.CompanyRepository.EditProfile(
                    user_id, sizeCategory, company_name, description, sector, website_link, country_headquarters
                );

                if (updatedCompany == null) return new CompanyError.FailedToGetCompanyInfo();

                return updatedCompany;
            });
        }

        public async Task<OneOf<Company, CompanyError>> GetById(int userId)
        {
            return await _transactionManager.Run<OneOf<Company, CompanyError>>(async (context) =>
        {
            var company = await context.CompanyRepository.GetByUserId(userId);

            if (company == null)
            {
                return new CompanyError.CompanyNotFound();
            }

            return company;
        });
        }


        public async Task<OneOf<PagedResponse<Company>, CompanyError>> Search(string? Name, string? sector, string? CompanySize, string? CountryHeadquarters, int Page, int PageSize)
        {
            Page = Page <= 0 ? 1 : Page;
            PageSize = PageSize <= 0 ? 10 : (PageSize > 100 ? 100 : PageSize);

            if (Name != null)
            {
                if (!_companiesDomain.IsSafeText(Name))
                    return new CompanyError.InvalidSectorName();
            }
           
            if (sector != null)
            {
                if (!_companiesDomain.IsSafeText(sector))
                    return new CompanyError.InvalidSectorName();
            }

            return await _transactionManager.Run<OneOf<PagedResponse<Company>, CompanyError>>(async (context) =>
            {

                var results = await context.CompanyRepository.Search(Name, sector, CompanySize, CountryHeadquarters, Page, PageSize);

                if (results == null)
                    return new CompanyError.FailedToGetCompanyInfo();

                return results;
            });
        }
    }
}