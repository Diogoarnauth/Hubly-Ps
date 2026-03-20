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
            // Validação de Domínio única agora

            /*if (!_creatorsDomain.IsValidArtisticName(artisticName)) 
                return new CreatorError.InvalidArtisticName();*/

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
                    CompanySize = company_size,
                    WebsiteLink = website_link,
                    CountryHeadquarters = country_headquarters
                };

                await context.CompanyRepository.RegisterCompany(newCompany);

                return newCompany;
            });
        }
        public async Task<OneOf<bool, CompanyError>> EditProfile(int user_id, int company_size, string company_name, string description, string sector, string website_link, string country_headquarters)
        {
            if (!_companiesDomain.IsValidSector(sector)) return new CompanyError.InvalidSectorName();

            return await _transactionManager.Run<OneOf<bool, CompanyError>>(async (context) =>
            {
                var company = await context.CompanyRepository.GetByUserId(user_id);
                if (company == null) return new CompanyError.FailedToGetCompanyInfo();
                
                await context.CompanyRepository.EditProfile(user_id, company_size, company_name, description, sector, website_link, country_headquarters);
                return true;
            });
        }
    }
}