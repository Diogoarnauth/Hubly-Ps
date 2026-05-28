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
    public class TokenServiceFixture
    {
        public Mock<ITokenEncoder> TokenEncoder { get; private set; }
        public Mock<IConfiguration> Configuration { get; private set; }
        public Mock<ITokenRepository> TokenRepository { get; private set; }
        public Mock<ITransactionContext> TransactionContext { get; private set; }
        public readonly TokenService TokenService;


        public readonly string TestToken = "test_token";
        public readonly string HashedToken = "hashed_token";
        public readonly int UserId = 1;
        public readonly Token ValidToken;

        public TokenServiceFixture()
        {
            TokenEncoder = new Mock<ITokenEncoder>();
            Configuration = new Mock<IConfiguration>();
            TokenRepository = new Mock<ITokenRepository>();
            TransactionContext = new Mock<ITransactionContext>();

            SetupConfigurationMock();

            SetupTransactionContext();

            ValidToken = new Token
            {
                UserId = UserId,
                TokenValidation = HashedToken,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                LastUsedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            TokenService = new TokenService(
                 TokenEncoder.Object,
                Configuration.Object,
                TokenRepository.Object
            );

            SetupDefaultMocks();
        }

        private void SetupConfigurationMock()
        {
            var sizeInBytesSection = new Mock<IConfigurationSection>();
            sizeInBytesSection.Setup(s => s.Value).Returns("32");

            var expirationTimeSection = new Mock<IConfigurationSection>();
            expirationTimeSection.Setup(s => s.Value).Returns("3600");

            var tokenSettingsSection = new Mock<IConfigurationSection>();

            Configuration.Setup(c => c.GetSection("TokenSettings:SizeInBytes"))
                .Returns(sizeInBytesSection.Object);
            Configuration.Setup(c => c.GetSection("TokenSettings:ExpirationTime"))
                .Returns(expirationTimeSection.Object);
        }

        private void SetupTransactionContext()
        {
            TransactionContext.Setup(tc => tc.TokenRepository).Returns(TokenRepository.Object);
        }

        public void ResetMocks()
        {
            TokenEncoder.Reset();
            TokenRepository.Reset();
            TransactionContext.Reset();
            SetupDefaultMocks();

            SetupTransactionContext();   

        }

        private void SetupDefaultMocks()
        {
            TokenEncoder.Setup(x => x.CreateValidationInformation(TestToken))
                .Returns(HashedToken);

            TokenRepository.Setup(x => x.GetTokenByValidation(HashedToken))
                .ReturnsAsync(ValidToken);
            TokenRepository.Setup(x => x.DeleteToken(UserId, HashedToken))
                .ReturnsAsync(true);
            TokenRepository.Setup(x => x.RefreshToken(HashedToken))
                .ReturnsAsync(true);
            TokenRepository.Setup(x => x.GetTokenByUserId(UserId))
                .ReturnsAsync(ValidToken);
            TokenRepository.Setup(x => x.DeleteTokenByValidation(It.IsAny<string>()))
                .ReturnsAsync(true);
        }

        public void SetupTokenValidationFails()
        {
            TokenRepository.Setup(x => x.GetTokenByValidation(HashedToken))
                .ReturnsAsync((Token?)null);
        }

        public void SetupTokenDeletionFails()
        {
            TokenRepository.Setup(x => x.DeleteTokenByValidation(It.IsAny<string>()))
                .ReturnsAsync(false);
        }

        public void SetupForNewTokenCreation()
        {
            TokenRepository.Setup(x => x.GetTokenByUserId(UserId))
                .ReturnsAsync((Token?)null);
            TokenRepository.Setup(x => x.CreateToken(It.IsAny<Token>()))
                .ReturnsAsync(true);
        }

        public void SetupForExistingTokenReplacement()
        {
            var existingToken = new Token
            {
                UserId = UserId,
                TokenValidation = "old_token",
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 1000,
                LastUsedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 1000
            };

            TokenRepository.Setup(x => x.GetTokenByUserId(UserId))
                .ReturnsAsync(existingToken);
            TokenRepository.Setup(x => x.DeleteToken(UserId, existingToken.TokenValidation))
                .ReturnsAsync(true);
            TokenRepository.Setup(x => x.CreateToken(It.IsAny<Token>()))
                .ReturnsAsync(true);
        }


    }
}