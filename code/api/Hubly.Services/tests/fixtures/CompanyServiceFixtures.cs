using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Services;
using Hubly.api.Services.Interfaces;
using Moq;
using Microsoft.Extensions.Configuration;
using Hubly.api.Services.Problems;
using OneOf;


namespace Hubly.api.Services.Fixtures
{
    public class CompanyServiceFixture
    {
        public readonly Mock<ICompanyRepository> CompanyRepository;
        public readonly Mock<ICreatorRepository> CreatorRepository;
        public readonly Mock<IHistoryRepository> HistoryRepository;
        public readonly Mock<ITransactionManager> TransactionManager;
        public readonly Mock<ITransactionContext> TransactionContext;
        public readonly Mock<IConfiguration> Configuration;

        public readonly CompaniesDomainConfig CompaniesDomainConfig;
        public readonly CompaniesDomain CompaniesDomain;

        public readonly CompanyService CompanyService;

        public readonly int UserId = 1;
        public readonly string CompanyName = "TestCompany";
        public readonly string Description = "Test Description";
        public readonly string WebsiteLink = "https://www.testcompany.com";
        public readonly string CountryHeadquarters = "Portugal";
        public readonly Company TestCompany;

        public CompanyServiceFixture()
        {
            CompanyRepository = new Mock<ICompanyRepository>();
            CreatorRepository = new Mock<ICreatorRepository>();
            HistoryRepository = new Mock<IHistoryRepository>();
            TransactionManager = new Mock<ITransactionManager>();
            TransactionContext = new Mock<ITransactionContext>();
            Configuration = new Mock<IConfiguration>();

            CompaniesDomainConfig = new CompaniesDomainConfig();

            CompaniesDomain = new CompaniesDomain(CompaniesDomainConfig);

            SetupConfigurationMock();

            SetupTransactionContext();

            TestCompany = new Company
            {
                Id = UserId,
                CompanyName = CompanyName,
                IsVerified = false,
                Description = Description,
                CompanySize = "0 a 100",
                WebsiteLink = WebsiteLink,
                CountryHeadquarters = CountryHeadquarters,
            };

            SetupTransactionManager();

            CompanyService = new CompanyService(
                TransactionManager.Object,
                Configuration.Object,
                CompaniesDomain
            );

            SetupDefaultMocks();
        }

        private void SetupConfigurationMock()
        {
            Configuration
                .Setup(c => c.GetSection(It.IsAny<string>()))
                .Returns(new Mock<IConfigurationSection>().Object);
        }

        private void SetupTransactionContext()
        {
            TransactionContext.Setup(tc => tc.CreatorRepository).Returns(CreatorRepository.Object);
            TransactionContext.Setup(tc => tc.CompanyRepository).Returns(CompanyRepository.Object);
            TransactionContext.Setup(tc => tc.HistoryRepository).Returns(HistoryRepository.Object);
        }

        private void SetupTransactionManager()
        {
            // 1. OneOf<Company, CompanyError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<Company, CompanyError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<Company, CompanyError>>>>(
                    async func => await func(TransactionContext.Object));

            // 2.OneOf<PagedResponse<Company>, CompanyError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<PagedResponse<Company>, CompanyError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<PagedResponse<Company>, CompanyError>>>>(
                    async func => await func(TransactionContext.Object));

            // 3. OneOf<List<Company>, CompanyError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<List<Company>, CompanyError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<List<Company>, CompanyError>>>>(
                    async func => await func(TransactionContext.Object));
        }

        public void ResetMocks()
        {
            CompanyRepository.Reset();
            CreatorRepository.Reset();
            HistoryRepository.Reset();
            TransactionManager.Reset();
            TransactionContext.Reset();

            SetupTransactionContext();
            
            SetupTransactionManager();
            SetupConfigurationMock();
            SetupDefaultMocks(); ;
        }

        private void SetupDefaultMocks()
        {
            CompanyRepository.Setup(x => x.ExistsByUserId(UserId)).ReturnsAsync(false);
        }

        public void SetupGetSectorByName(List<string> input, List<Sector> result) =>
            CompanyRepository.Setup(x => x.GetSectorByName(input)).ReturnsAsync(result);

        public void SetupCompanyExists(int userId, bool exists) =>
            CompanyRepository.Setup(x => x.ExistsByUserId(userId)).ReturnsAsync(exists);

        public void SetupCreatorExists(int userId, bool exists) =>
            CreatorRepository.Setup(x => x.ExistsByUserId(userId)).ReturnsAsync(exists);

        public void SetupGetByUserId(int userId, Company? company) =>
            CompanyRepository.Setup(x => x.GetByUserId(userId)).ReturnsAsync(company);

        public void SetupTrendingCompanies(List<Company>? companies)
        {
            HistoryRepository.Setup(x => x.GetTopTrendingCompanies(It.IsAny<int>()))
                .ReturnsAsync(companies);
        }

        public void SetupUserInterests(CreatorInterestProfile interests)
        {
            HistoryRepository.Setup(x => x.GetCreatorInterests(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(interests);
        }
    }

}