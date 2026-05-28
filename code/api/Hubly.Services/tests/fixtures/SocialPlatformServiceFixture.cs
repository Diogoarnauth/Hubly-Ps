using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Services;
using Hubly.api.Services.Interfaces;
using Moq;
using Microsoft.Extensions.Configuration;
using OneOf;

namespace Hubly.api.Services.Fixtures
{
    public class SocialPlatformServiceFixture
    {
        public readonly Mock<ISocialPlatformRepository> SocialPlatformRepository;
        public readonly Mock<ITransactionManager> TransactionManager;
        public readonly Mock<ITransactionContext> TransactionContext;
        public readonly Mock<IConfiguration> Configuration;
        
        public readonly SocialPlatformService SocialPlatformService;

        public SocialPlatformServiceFixture()
        {
            SocialPlatformRepository = new Mock<ISocialPlatformRepository>();
            TransactionManager = new Mock<ITransactionManager>();
            TransactionContext = new Mock<ITransactionContext>();
            Configuration = new Mock<IConfiguration>();

            SetupConfigurationMock();

            SetupTransactionContext();

            SetupTransactionManager();

            SocialPlatformService = new SocialPlatformService(
                TransactionManager.Object,
                Configuration.Object
            );

            SetupDefaultMocks();
        }

        private void SetupConfigurationMock()
        {
            Configuration.Setup(c => c.GetSection(It.IsAny<string>()))
                .Returns(new Mock<IConfigurationSection>().Object);
        }

        private void SetupTransactionContext()
        {
            TransactionContext.Setup(tc => tc.SocialPlatformRepository)
                .Returns(SocialPlatformRepository.Object);
        }

        private void SetupTransactionManager()
        {
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<List<SocialPlatform>, SocialPlatformError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<List<SocialPlatform>, SocialPlatformError>>>>(
                    async func => await func(TransactionContext.Object));
        }

        public void ResetMocks()
        {
            SocialPlatformRepository.Reset();
            TransactionManager.Reset();
            TransactionContext.Reset();

            SetupTransactionContext();

            SetupTransactionManager();
            SetupConfigurationMock();
            SetupDefaultMocks();
        }

        private void SetupDefaultMocks()
        {
            var allPlatforms = new List<SocialPlatform> 
            { 
                new SocialPlatform { Id = 1, NamePlatform = "YouTube" },
                new SocialPlatform { Id = 2, NamePlatform = "Instagram" },
                new SocialPlatform { Id = 3, NamePlatform = "Facebook" },
                new SocialPlatform { Id = 4, NamePlatform = "X" },
                new SocialPlatform { Id = 5, NamePlatform = "Telegram" },
                new SocialPlatform { Id = 6, NamePlatform = "TikTok" }
            };

            SocialPlatformRepository.Setup(x => x.GetAll())
                .ReturnsAsync(allPlatforms);
        }

        public void SetupGetAllPlatformsFails()
        {
            SocialPlatformRepository.Setup(x => x.GetAll())
                .ReturnsAsync((List<SocialPlatform>)null!);
        }

        
    }
}