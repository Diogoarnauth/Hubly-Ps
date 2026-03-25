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

        public async Task<OneOf<Company, CompanyError>> Register(int userId, int company_size, string company_name, string description, string sector, string subSector, string website_link, string country_headquarters)
        {
            if (!_companiesDomain.IsValidWebsite(website_link)) return new CompanyError.InvalidWebSiteLink();
            if (!_companiesDomain.IsValidCountry(country_headquarters)) return new CompanyError.InvalidCountryHeadquarters();

            string sizeCategory = _companiesDomain.ConvertCompanySize(company_size);

            var result = await _transactionManager.Run<OneOf<Company, CompanyError>>(async (context) =>
            {
                var sectorId = await context.CompanyRepository.GetSectorIdByName(sector);
                if (sectorId == null) return new CompanyError.InvalidSectorName();

                int? subSectorId = null;
                if (!string.IsNullOrWhiteSpace(subSector))
                {
                    subSectorId = await context.CompanyRepository.GetSubSectorIdByName(sectorId.Value, subSector);
                    if (subSectorId == null) return new CompanyError.InvalidSubSectorName();
                }

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
                    SectorId = sectorId.Value,
                    SubSectorId = subSectorId,
                    CompanySize = sizeCategory,
                    WebsiteLink = website_link,
                    CountryHeadquarters = country_headquarters
                };

                await context.CompanyRepository.RegisterCompany(newCompany);

                return newCompany;
            });

            if (result.IsT0)
            {
                var company = result.AsT0;

                company.Sector = new Sector { Id = company.SectorId, SectorName = sector };
                if (company.SubSectorId.HasValue)
                {
                    company.SubSector = new SubSector { Id = company.SubSectorId.Value, SubSectorName = subSector };
                }
            }

            return result;
        }
        public async Task<OneOf<Company, CompanyError>> EditProfile(int user_id, int company_size, string company_name, string description, string sector, string subSector, string website_link, string country_headquarters)
        {
            // 1. Validações iniciais
            if (!_companiesDomain.IsSafeText(sector)) return new CompanyError.InvalidSectorName();
            if (!_companiesDomain.IsValidWebsite(website_link)) return new CompanyError.InvalidWebSiteLink();
            if (!_companiesDomain.IsValidCountry(country_headquarters)) return new CompanyError.InvalidCountryHeadquarters();

            string sizeCategory = _companiesDomain.ConvertCompanySize(company_size);

            // 2. Execução da transação na BD
            var result = await _transactionManager.Run<OneOf<Company, CompanyError>>(async (context) =>
            {
                var sectorId = await context.CompanyRepository.GetSectorIdByName(sector);
                if (sectorId == null) return new CompanyError.InvalidSectorName();

                int? subSectorId = null;
                if (!string.IsNullOrWhiteSpace(subSector))
                {
                    subSectorId = await context.CompanyRepository.GetSubSectorIdByName(sectorId.Value, subSector);
                    if (subSectorId == null) return new CompanyError.InvalidSubSectorName();
                }

                var companyExists = await context.CompanyRepository.GetByUserId(user_id);
                if (companyExists == null) return new CompanyError.FailedToGetCompanyInfo();

                var updatedCompany = await context.CompanyRepository.EditProfile(
                    user_id, sizeCategory, company_name, description, sectorId.Value, subSectorId, website_link, country_headquarters
                );

                if (updatedCompany == null) return new CompanyError.FailedToGetCompanyInfo();

                return updatedCompany;
            });

            if (result.IsT0)
            {
                var company = result.AsT0;

                company.Sector = new Sector { Id = company.SectorId, SectorName = sector };
                if (company.SubSectorId.HasValue)
                {
                    company.SubSector = new SubSector { Id = company.SubSectorId.Value, SubSectorName = subSector };
                }

                return company;
            }

            return result;
        }

       public async Task<OneOf<Company, CompanyError>> GetById(int userId)
{
    var result = await _transactionManager.Run<OneOf<Company, CompanyError>>(async (context) =>
    {
        var company = await context.CompanyRepository.GetByUserId(userId);
        if (company == null) return new CompanyError.CompanyNotFound();

        return company;
    });

    return result;
}


        /*public async Task<OneOf<PagedResponse<Company>, CompanyError>> Search(string? Name, string? sector, string? CompanySize, string? CountryHeadquarters, int Page, int PageSize)
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
        }*/
    }
}