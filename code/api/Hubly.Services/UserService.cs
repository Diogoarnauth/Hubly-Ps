using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;


namespace Hubly.api.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IPasswordEncoder _passwordEncoder;

        private readonly ITransactionManager _transactionManager;
        private readonly UsersDomain _usersDomain;

        private readonly int _expiredHours;
        private readonly int _codeLength;


        public UserService(
            ITokenService tokenService,
            IPasswordEncoder passwordEncoder,
            ITransactionManager transactionManager,
            IEmailService emailService,
            IConfiguration configuration,
            UsersDomain usersDomain
        )
        {
            _emailService = emailService;
            _tokenService = tokenService;
            _passwordEncoder = passwordEncoder;
            _transactionManager = transactionManager;
            _usersDomain = usersDomain;
            _expiredHours = int.Parse(configuration.GetSection("EmailSettings:ConfirmationCodeExpiryHours").Value ?? "24");
            _codeLength = int.Parse(configuration.GetSection("EmailSettings:ConfirmationCodeLength").Value ?? "6");

        }

        public async Task<OneOf<User, UserError>> Register(string userName, string email, string password)
        {
            if (!_usersDomain.IsValidUsername(userName)) return new UserError.InvalidName();
            if (!_usersDomain.ValidationEmail(email)) return new UserError.InvalidEmail();
            if (!_usersDomain.IsSafePassword(password)) return new UserError.InvalidPassword();

            return await _transactionManager.Run<OneOf<User, UserError>>(async (context) =>
            {
                if (await context.UserRepository.UserExistsWithEmail(email)) return new UserError.EmailAlreadyExists();

                var passwordInfo = _passwordEncoder.createValidationInformation(password);

                var newUser = new User
                {
                    Name = userName,
                    Email = email,
                    PasswordValidation = passwordInfo,
                    IsEmailConfirmed = false,
                    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                bool created = await context.UserRepository.CreateUser(newUser);
                if (!created)
                {
                    return new UserError.FailedUserCreation();
                }
                await GenerateConfirmationCode(newUser.Id, context);
                return newUser;
            });
        }

        public async Task<OneOf<string, UserError>> Token(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email)) return new UserError.InvalidEmail();
            if (string.IsNullOrWhiteSpace(password)) return new UserError.InvalidPassword();

            return await _transactionManager.Run<OneOf<string, UserError>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserByEmail(email);

                if (user == null || !_passwordEncoder.Verify(password, user.PasswordValidation))
                {
                    return new UserError.InvalidCredentials();
                }

                var existingTokens = await context.TokenRepository.GetTokensByUser(user.Id);
                int maxTokens = _usersDomain.MaxTokensPerUser;

                if (existingTokens.Count >= maxTokens)
                {
                    var oldestToken = existingTokens
                        .OrderBy(t => t.LastUsedAt)
                        .FirstOrDefault();

                    if (oldestToken != null)
                    {
                        await context.TokenRepository.DeleteTokenByValidation(oldestToken.TokenValidation);
                    }
                }

                var rawToken = _tokenService.GenerateTokenValue();
                var validationInfo = _tokenService.CreateTokenValidationInformation(rawToken);

                var newToken = new Token
                {
                    TokenValidation = validationInfo,
                    UserId = user.Id,
                    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    LastUsedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                await context.TokenRepository.CreateToken(newToken);

                return rawToken;
            });
        }
        public async Task<OneOf<string, UserError>> Logout(string tokenValue)
        {
            return await _transactionManager.Run<OneOf<string, UserError>>(async (context) =>
            {
                await _tokenService.DeleteToken(tokenValue, context);
                return "Logout successful";
            });
        }


        public async Task<OneOf<User, UserError>> GetUserInfo(int userId)
        {
            return await _transactionManager.Run<OneOf<User, UserError>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserById(userId);
                if (user == null) return new UserError.FailedToGetUserInfo();

                return user;
            });
        }

        public async Task<OneOf<bool, UserError>> CheckCreatorOrCompany(int userId)
        {
            try
            {
                return await _transactionManager.Run<bool>(async (context) =>
                {
                    var creator = await context.CreatorRepository.GetByUserId(userId);
                    if (creator != null) return true;

                    var company = await context.CompanyRepository.GetByUserId(userId);
                    if (company != null) return true;

                    return false;
                });
            }
            catch
            {
                return new UserError.FailedToGetUserInfo();
            }
        }

        public async Task<OneOf<bool, UserError>> EditUser(int userId, string newUsername)
        {
            if (!_usersDomain.IsValidUsername(newUsername)) return new UserError.InvalidName();

            return await _transactionManager.Run<OneOf<bool, UserError>>(async (context) =>
            {
                var userid = await context.UserRepository.GetUserById(userId);
                if (userid == null)
                {
                    return new UserError.FailedToGetUserInfo();
                }
                await context.UserRepository.EditUser(userId, newUsername);
                return true;
            });
        }


        public async Task<OneOf<string, UserError>> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            if (!_usersDomain.IsSafePassword(newPassword)) return new UserError.InvalidPassword();

            return await _transactionManager.Run<OneOf<string, UserError>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserById(userId);
                if (user == null) return new UserError.UserNotFound();

                if (!_passwordEncoder.Verify(oldPassword, user.PasswordValidation)) return new UserError.OldPasswordIsIncorrect();

                if (oldPassword == newPassword) return new UserError.NewPasswordCannotBeTheSameAsTheOldPassword();

                var newPasswordHash = _passwordEncoder.createValidationInformation(newPassword);
                await context.UserRepository.ChangePassword(userId, newPasswordHash);

                return "Password changed successfully";
            });
        }

        public async Task<OneOf<string, UserError>> ResendEmailConfirmation(string email)
        {
            return await _transactionManager.Run<OneOf<string, UserError>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserByEmail(email);
                if (user == null)
                {
                    return new UserError.UserNotFound();
                }
                if (user.IsEmailConfirmed)
                {
                    return new UserError.EmailAlreadyConfirmed();
                }
                await GenerateConfirmationCode(user.Id, context);
                return "Email confirmation sent";
            });
        }

        //
        public async Task<OneOf<string, UserError>> GenerateConfirmationCode(int userId, ITransactionContext context)
        {
            var user = await context.UserRepository.GetUserById(userId);
            if (user == null)
            {
                return new UserError.UserNotFound();
            }
            string confirmationCode = GenerateNumericCode(_codeLength);
            await context.EmailConfirmationRepository.CreateConfirmationCodeAsync(userId, confirmationCode, _expiredHours);
            await _emailService.SendConfirmationEmailAsync(user.Email, user.Name, confirmationCode);
            return confirmationCode;
        }
        //
        public async Task<OneOf<bool, UserError>> VerifyConfirmationCodeAsync(string email, string code)
        {
            return await _transactionManager.Run<OneOf<bool, UserError>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserByEmail(email);
                if (user == null)
                {
                    return new UserError.UserNotFound();
                }
                var confirmationCode = await context.EmailConfirmationRepository.GetConfirmationCodeAsync(code);

                if (confirmationCode == null || confirmationCode.UserId != user.Id || confirmationCode.Used || confirmationCode.ExpiresAt < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    return new UserError.InvalidConfirmationCode();
                }
                await context.EmailConfirmationRepository.MarkConfirmationCodeAsUsedAsync(confirmationCode.Id);

                bool confirmed = await context.EmailConfirmationRepository.ConfirmUserEmailAsync(user.Id);
                if (!confirmed)
                {
                    return new UserError.FailedToConfirmEmail();
                }
                return true;
            });
        }
        //
        public async Task<OneOf<bool, UserError>> ResendConfirmationCodeAsync(int userId)
        {
            return await _transactionManager.Run<OneOf<bool, UserError>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserById(userId);
                if (user == null)
                {
                    return new UserError.UserNotFound();
                }
                if (user.IsEmailConfirmed)
                {
                    return new UserError.EmailAlreadyConfirmed();
                }
                if (await context.EmailConfirmationRepository.CodeExists(userId))
                {
                    return new UserError.CodeAlreadyExists();
                }
                var confirmationResult = await GenerateConfirmationCode(userId, context);

                return confirmationResult.Match<OneOf<bool, UserError>>(
                    code => true,
                    error => error
                );
            });
        }

        public async Task<OneOf<PagedResponse<ProfileViewHistory>, UserError>> GetHistory(int userId, int page = 1, int pageSize = 20)
        {
            return await _transactionManager.Run<OneOf<PagedResponse<ProfileViewHistory>, UserError>>(async (context) =>
            {
                var history = await context.HistoryRepository.GetUserHistory(userId, page, pageSize);

                return history;
            });
        }

        public async Task<OneOf<User, UserError>> GetFullCreatorProfile(int targetCreatorId, int viewerId, int? coWorkerId)
        {
            return await _transactionManager.Run<OneOf<User, UserError>>(async (context) =>
            {
                var user = await context.UserRepository.GetFullUserById(targetCreatorId);

                if (user == null) return new UserError.UserNotFound();

                if (targetCreatorId != viewerId)
                {
                    try
                    {
                        var primaryProfileId = user.Creator?.SocialProfiles?.FirstOrDefault()?.Id;

                        if (primaryProfileId.HasValue)
                        {
                            var historyEntry = new ProfileViewHistory
                            {
                                ViewerUserId = viewerId,
                                ViewedSocialProfileId = primaryProfileId.Value,
                                ViewedAt = DateTime.UtcNow
                            };

                            await context.HistoryRepository.AddView(historyEntry);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Hubly: Erro ao gravar histórico: {ex.Message}");
                    }
                }

                return user;
            });
        }
         public async Task<OneOf<User, UserError>> GetFullCompanyProfile(int targetCompanyId, int viewerId, int? coWorkerId)
        {
            return await _transactionManager.Run<OneOf<User, UserError>>(async (context) =>
           {
               var user = await context.UserRepository.GetFullUserCompanyById(targetCompanyId);

               if (user == null) return new UserError.UserNotFound();

               var isOwner = targetCompanyId == viewerId;

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

               return user;
           });
        }


        private string GenerateNumericCode(int length)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] data = new byte[length];
            rng.GetBytes(data);


            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)('0' + (data[i] % 10));
            }
            return new string(chars);
        }


    }
}