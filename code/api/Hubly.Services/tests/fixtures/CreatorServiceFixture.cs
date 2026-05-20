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
    public class CreatorServiceFixture
    {
        public readonly Mock<ICreatorRepository> CreatorRepository;
        public readonly Mock<ICompanyRepository> CompanyRepository; 
        public readonly Mock<IHistoryRepository> HistoryRepository;
        public readonly Mock<ISocialPlatformRepository> SocialPlatformRepository;
        public readonly Mock<ICreatorSocialRepository> CreatorSocialRepository;
        public readonly Mock<ITransactionManager> TransactionManager;
        public readonly Mock<ITransactionContext> TransactionContext;
        public readonly Mock<IConfiguration> Configuration;

        public readonly CreatorsDomainConfig CreatorsDomainConfig;
        public readonly CreatorsDomain CreatorsDomain;

        public readonly CreatorService CreatorService;


        public readonly int UserId = 1;
        public readonly string ArtisticName = "ArtisticTestName";
        public readonly string AvailabilityStatus = "AVAILABLE";
        public readonly Creator TestCreator;

        public CreatorServiceFixture()
        {
            CreatorRepository = new Mock<ICreatorRepository>();
            CompanyRepository = new Mock<ICompanyRepository>();
            HistoryRepository = new Mock<IHistoryRepository>();
            SocialPlatformRepository = new Mock<ISocialPlatformRepository>();
            CreatorSocialRepository = new Mock<ICreatorSocialRepository>();
            TransactionManager = new Mock<ITransactionManager>();
            TransactionContext = new Mock<ITransactionContext>();
            Configuration = new Mock<IConfiguration>();

            CreatorsDomainConfig = new CreatorsDomainConfig
            {
                MinArtitisticNameLength = 2
            };

            CreatorsDomain = new CreatorsDomain(CreatorsDomainConfig);

            SetupConfigurationMock();

            TransactionContext.Setup(tc => tc.CreatorRepository).Returns(CreatorRepository.Object);
            TransactionContext.Setup(tc => tc.CompanyRepository).Returns(CompanyRepository.Object);
            TransactionContext.Setup(tc => tc.CreatorSocialRepository).Returns(CreatorSocialRepository.Object);

            TestCreator = new Creator
            {
                Id = UserId,
                ArtisticName = ArtisticName,
                IsVerified = false,
                AvailabilityStatus = AvailabilityStatus,
                GlobalRating = 0,
                RatingsCount = 0,
                ChatsStartedCount = 0,
                ChatsRespondedCount = 0
            };

            SetupTransactionManager();

            CreatorService = new CreatorService(
                TransactionManager.Object,
                Configuration.Object,
                CreatorsDomain
            );

            SetupDefaultMocks();
        }


        private void SetupConfigurationMock()
        {
            Configuration
                .Setup(c => c.GetSection(It.IsAny<string>()))
                .Returns(new Mock<IConfigurationSection>().Object);
        }

        private void SetupTransactionManager()
        {
            // 1. OneOf<bool>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<bool>>>()))
                .Returns<Func<ITransactionContext, Task<bool>>>(
                    async func => await func(TransactionContext.Object));

            // 2. OneOf<Creator>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<Creator>>>()))
                .Returns<Func<ITransactionContext, Task<Creator>>>(
                    async func => await func(TransactionContext.Object));

            // 3. OneOf<int?>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<int?>>>()))
                .Returns<Func<ITransactionContext, Task<int?>>>(
                    async func => await func(TransactionContext.Object));
            
            // 4. OneOf<CreatorSocialProfile>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<CreatorSocialProfile>>>()))
                .Returns<Func<ITransactionContext, Task<CreatorSocialProfile>>>(
                    async func => await func(TransactionContext.Object));

            // 5. OneOf<bool, CreatorError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<bool, CreatorError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<bool, CreatorError>>>>(
                    async func => await func(TransactionContext.Object));

            // 6. OneOf<Creator, CreatorError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<Creator, CreatorError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<Creator, CreatorError>>>>(
                    async func => await func(TransactionContext.Object));

            // 7. OneOf<int?, CreatorError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<int?, CreatorError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<int?, CreatorError>>>>(
                    async func => await func(TransactionContext.Object));

            // 8. OneOf<(CreatorSocialProfile, bool), CreatorError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<(CreatorSocialProfile, bool), CreatorError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<(CreatorSocialProfile, bool), CreatorError>>>>(
                    async func => await func(TransactionContext.Object));

            // 9. OneOf<CreatorSocialProfile, CreatorError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task <OneOf<CreatorSocialProfile, CreatorError>>>> ()))
                .Returns<Func<ITransactionContext, Task<OneOf<CreatorSocialProfile, CreatorError>>>>(async func => await func(TransactionContext.Object));

            // 10. OneOf<List<Sector>, CreatorError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny < Func < ITransactionContext, Task < OneOf<List<Sector>, CreatorError>>>> ()))
                .Returns<Func<ITransactionContext, Task<OneOf<List<Sector>, CreatorError>>>>(async func => await func(TransactionContext.Object));

            // 12. OneOf<PagedResponse<CreatorSocialProfile>, CreatorError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny < Func < ITransactionContext, Task < OneOf<PagedResponse<CreatorSocialProfile>, CreatorError>>>> ()))
                .Returns<Func<ITransactionContext, Task<OneOf<PagedResponse<CreatorSocialProfile>, CreatorError>>>>(async func => await func(TransactionContext.Object));

            // 13. OneOf<List<CreatorSocialProfile>, CreatorError>
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny < Func < ITransactionContext, Task < OneOf<List<CreatorSocialProfile>, CreatorError>>>> ()))
                .Returns<Func<ITransactionContext, Task<OneOf<List<CreatorSocialProfile>, CreatorError>>>>(async func => await func(TransactionContext.Object));
        }

        public void ResetMocks()
        {
            CreatorRepository.Reset();
            CompanyRepository.Reset(); 
            HistoryRepository.Reset();
            SocialPlatformRepository.Reset();
            CreatorSocialRepository.Reset();
            TransactionManager.Reset();
            TransactionContext.Reset();

            TransactionContext.Setup(tc => tc.CreatorRepository).Returns(CreatorRepository.Object);
            TransactionContext.Setup(tc => tc.CompanyRepository).Returns(CompanyRepository.Object);
            TransactionContext.Setup(tc => tc.CreatorSocialRepository).Returns(CreatorSocialRepository.Object);

            SetupTransactionManager();
            SetupConfigurationMock();
            SetupDefaultMocks();
        }

        private void SetupDefaultMocks()
        {
            CreatorRepository.Setup(x => x.ExistsByUserId(UserId)).ReturnsAsync(false);
        }

        private void SetupTransactionContext()
        {
            TransactionContext.Setup(tc => tc.CreatorRepository).Returns(CreatorRepository.Object);
            TransactionContext.Setup(tc => tc.HistoryRepository).Returns(HistoryRepository.Object);
            TransactionContext.Setup(tc => tc.SocialPlatformRepository).Returns(SocialPlatformRepository.Object);
            TransactionContext.Setup(tc => tc.CreatorSocialRepository).Returns(CreatorSocialRepository.Object);  
        }

        public void SetupCreatorAlreadyExists()
        {
            CreatorRepository.Setup(x => x.ExistsByUserId(UserId)).ReturnsAsync(true);
        }

        public void SetupRegisterSuccess()
        {
            CreatorRepository.Setup(x => x.ExistsByUserId(UserId)).ReturnsAsync(false);
            CompanyRepository.Setup(x => x.ExistsByUserId(UserId)).ReturnsAsync(false);
            CreatorRepository.Setup(x => x.RegisterCreator(It.IsAny<Creator>())).ReturnsAsync(true);
        }    

        
    }
}