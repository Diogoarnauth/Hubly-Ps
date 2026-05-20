using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Services;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OneOf;

namespace Hubly.api.Services.Fixtures
{
    public class UserServiceFixture
    {
        public readonly Mock<IUserRepository> UserRepository;
        public readonly Mock<ITokenRepository> TokenRepository;
        public readonly Mock<ITokenService> TokenService;
        public readonly Mock<IPasswordEncoder> PasswordEncoder;
        public readonly Mock<IEmailService> EmailService;
        public readonly Mock<IConfiguration> Configuration;
        public readonly Mock<ILogger<UserService>> Logger;
        public readonly Mock<ITransactionManager> TransactionManager;
        public readonly Mock<ITransactionContext> TransactionContext;
        
        public readonly UsersDomainConfig UsersDomainConfig; 
        public readonly UsersDomain UsersDomain; 
        
        public readonly UserService UserService;

        public readonly string ValidEmail = "test@example.com";
        public readonly string ValidPassword = "@StrongPassword123!";
        public readonly string ValidUsername = "testuser";
        public readonly string HashedPassword = "hashed_password";
        public readonly string HashedNewPassword = "hashed_new_password";
        public readonly string TokenValue = "test_token";
        public readonly string TokenValidation = "token_validation";
        public readonly int UserId = 1;
        public readonly User TestUser;

        public UserServiceFixture()
        {
            UserRepository = new Mock<IUserRepository>();
            TokenRepository = new Mock<ITokenRepository>();
            TokenService = new Mock<ITokenService>();
            PasswordEncoder = new Mock<IPasswordEncoder>();
            EmailService = new Mock<IEmailService>();
            Configuration = new Mock<IConfiguration>();
            Logger = new Mock<ILogger<UserService>>();
            TransactionManager = new Mock<ITransactionManager>();
            TransactionContext = new Mock<ITransactionContext>();

            UsersDomainConfig = new UsersDomainConfig
            {
                MaxTokensPerUser = 3,
                MinUsernameLength = 3,
                MinPasswordLength = 8
            };

            UsersDomain = new UsersDomain(UsersDomainConfig);

            SetupConfigurationMock();

            TransactionContext.Setup(tc => tc.UserRepository).Returns(UserRepository.Object);
            TransactionContext.Setup(tc => tc.EmailConfirmationRepository).Returns(new Mock<IEmailConfirmationRepository>().Object);

            TestUser = new User
            {
                Id = UserId,
                Name = ValidUsername,
                Email = ValidEmail,
                PasswordValidation = HashedPassword,
                IsEmailConfirmed = true,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            SetupTransactionManager();

            UserService = new UserService(
                TokenService.Object,
                PasswordEncoder.Object,
                TransactionManager.Object,
                EmailService.Object,
                Configuration.Object,
                UsersDomain
            );

            SetupDefaultMocks();
        }

        private void SetupConfigurationMock()
        {
            Configuration.Setup(c => c["UsersDomainConfig:MaxTokensPerUser"]).Returns("3");
            Configuration.Setup(c => c["UsersDomainConfig:MinUsernameLength"]).Returns("3");
            Configuration.Setup(c => c["UsersDomainConfig:MinPasswordLength"]).Returns("8");

            var domainSection = new Mock<IConfigurationSection>();
            domainSection.Setup(s => s["MaxTokensPerUser"]).Returns("3");
            domainSection.Setup(s => s["MinUsernameLength"]).Returns("3");
            domainSection.Setup(s => s["MinPasswordLength"]).Returns("8");
            Configuration.Setup(c => c.GetSection("UsersDomainConfig")).Returns(domainSection.Object);

            var expirySection = new Mock<IConfigurationSection>();
            expirySection.Setup(s => s.Value).Returns("24");

            var lengthSection = new Mock<IConfigurationSection>();
            lengthSection.Setup(s => s.Value).Returns("6");

            Configuration.Setup(c => c.GetSection("EmailSettings:ConfirmationCodeExpiryHours")).Returns(expirySection.Object);
            Configuration.Setup(c => c.GetSection("EmailSettings:ConfirmationCodeLength")).Returns(lengthSection.Object);
        }

        private void SetupTransactionManager()
        {
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<bool>>>()))
                .Returns<Func<ITransactionContext, Task<bool>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<User>>>()))
                .Returns<Func<ITransactionContext, Task<User>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<string>>>()))
                .Returns<Func<ITransactionContext, Task<string>>>(
                    async func => await func(TransactionContext.Object));

            var transactionContextMock = new Mock<ITransactionContext>();
            transactionContextMock.Setup(c => c.UserRepository).Returns(UserRepository.Object);
            transactionContextMock.Setup(c => c.TokenRepository).Returns(TokenRepository.Object);
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<string, UserError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<string, UserError>>>>(
                    async func => await func(transactionContextMock.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<User, UserError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<User, UserError>>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<bool, UserError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<bool, UserError>>>>(
                    async func => await func(TransactionContext.Object));
        }

        public void ResetMocks()
        {
            UserRepository.Reset();
            TokenService.Reset();
            PasswordEncoder.Reset();
            EmailService.Reset();
            TransactionManager.Reset();
            TransactionContext.Reset();

            TransactionContext.Setup(tc => tc.UserRepository).Returns(UserRepository.Object);
            TransactionContext.Setup(tc => tc.EmailConfirmationRepository).Returns(new Mock<IEmailConfirmationRepository>().Object);

            SetupTransactionManager();
            SetupConfigurationMock();
            SetupDefaultMocks();
        }

        private void SetupDefaultMocks()
        {

            PasswordEncoder.Setup(x => x.Verify(ValidPassword, HashedPassword)).Returns(true);
            PasswordEncoder.Setup(x => x.Verify("PasswordSegura123", HashedPassword)).Returns(true);
            PasswordEncoder.Setup(x => x.Verify("PasswordInsegura123", HashedNewPassword)).Returns(true);

            PasswordEncoder.Setup(x => x.createValidationInformation(ValidPassword)).Returns(HashedPassword);
            PasswordEncoder.Setup(x => x.createValidationInformation("PasswordInsegura123")).Returns(HashedNewPassword);

            UserRepository.Setup(x => x.GetUserByEmail(ValidEmail)).ReturnsAsync(TestUser);

            TokenRepository.Setup(x => x.GetTokensByUser(UserId)).ReturnsAsync(new List<Token>());
            TokenService.Setup(x => x.GenerateTokenValue()).Returns(TokenValue);
            TokenService.Setup(x => x.CreateTokenValidationInformation(TokenValue)).Returns(TokenValidation);
            TokenService.Setup(x => x.CreateToken(UserId, TokenValidation, It.IsAny<ITransactionContext>())).ReturnsAsync(true);
            TokenService.Setup(x => x.DeleteToken(It.IsAny<string>(), It.IsAny<ITransactionContext>())).ReturnsAsync(true);

            UserRepository.Setup(x => x.UserExistsWithEmail(ValidEmail)).ReturnsAsync(false);
            UserRepository.Setup(x => x.CreateUser(It.IsAny<User>())).ReturnsAsync(true);

            UserRepository.Setup(x => x.GetUserById(UserId)).ReturnsAsync(TestUser);
            UserRepository.Setup(x => x.ChangePassword(UserId, HashedNewPassword)).Returns(Task.CompletedTask);
            UserRepository.Setup(x => x.EditUser(UserId, It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        public void SetupInvalidCredentials()
        {
            UserRepository.Setup(x => x.GetUserByEmail(ValidEmail)).ReturnsAsync(TestUser);
            PasswordEncoder.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        }

        public void SetupTokenUpdateFails()
        {
            TokenService.Setup(x => x.CreateToken(UserId, TokenValidation, It.IsAny<ITransactionContext>())).ReturnsAsync(false);
        }

        public void SetupUserWithEmailExists()
        {
            UserRepository.Setup(x => x.UserExistsWithEmail(ValidEmail)).ReturnsAsync(true);
        }

        public void SetupCreateUserFails()
        {
            UserRepository.Setup(x => x.CreateUser(It.IsAny<User>())).ReturnsAsync(false);
        }

        public void SetupRepositoryThrowsException()
        {
            UserRepository.Setup(x => x.GetUserByEmail(ValidEmail)).ThrowsAsync(new Exception("Database error"));
            UserRepository.Setup(x => x.UserExistsWithEmail(ValidEmail)).ThrowsAsync(new Exception("Database error"));
        }

        public void SetupTokenServiceThrowsException()
        {
            TokenService.Setup(x => x.DeleteToken(It.IsAny<string>(), It.IsAny<ITransactionContext>())).ThrowsAsync(new Exception("Token service error"));
        }

        public void SetupGetUserByIdSucceeds()
        {
            UserRepository.Setup(x => x.GetUserById(UserId)).ReturnsAsync(TestUser);
        }

        public void SetupGetUserByIdFails()
        {
            UserRepository.Setup(x => x.GetUserById(UserId)).ReturnsAsync((User)null);
        }

        public void SetupChangePasswordSucceeds()
        {
            PasswordEncoder.Setup(x => x.Verify("PasswordSegura123", HashedPassword)).Returns(true);
            PasswordEncoder.Setup(x => x.createValidationInformation("PasswordInsegura123")).Returns(HashedNewPassword);
            UserRepository.Setup(x => x.GetUserById(UserId)).ReturnsAsync(TestUser);
        }

        public void SetupChangePasswordOldPasswordIncorrect()
        {
            PasswordEncoder.Setup(x => x.Verify("wrongOldPassword", HashedPassword)).Returns(false);
            UserRepository.Setup(x => x.GetUserById(UserId)).ReturnsAsync(TestUser);
        }

        public void SetupChangePasswordSamePassword()
        {
            PasswordEncoder.Setup(x => x.Verify("PasswordSegura123", HashedPassword)).Returns(true);
            UserRepository.Setup(x => x.GetUserById(UserId)).ReturnsAsync(TestUser);
        }
    }
}