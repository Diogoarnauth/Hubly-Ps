using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;
using System.Data.Common;
using System.Linq;


namespace Hubly.api.Services
{
    public class UserService: IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IPasswordEncoder _passwordEncoder;

        private readonly ITransactionManager _transactionManager;
        private readonly UsersDomain _usersDomain;
    

    public UserService(
        ITokenService tokenService,
        IPasswordEncoder passwordEncoder,
        ITransactionManager transactionManager,
        UsersDomain usersDomain
    )
    {
        _tokenService = tokenService;
        _passwordEncoder = passwordEncoder;
        _transactionManager = transactionManager;
        _usersDomain = usersDomain;
    }

    public async Task<OneOf<User,UserError>> Register(string userName, string email, string password)
    {
        if (!_usersDomain.IsSafePassword(password)) return new UserError.InvalidPassword();
            
        if (!_usersDomain.IsValidUsername(userName)) return new UserError.InvalidName();

        if (!_usersDomain.ValidationEmail(email)) return new UserError.InvalidEmail();
            
        return await _transactionManager.Run<OneOf<User, UserError>>(async (context) =>
        {
            if (await context.UserRepository.UserExistsWithEmail(email)) return new UserError.EmailAlreadyExists();

        var passwordInfo = _passwordEncoder.createValidationInformation(password);
        
        var newUser = new User {
            Name = userName, 
            Email = email,
            PasswordValidation = passwordInfo,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        await context.UserRepository.CreateUser(newUser);
        
    
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

        var passwordHash = _passwordEncoder.createValidationInformation(password);

        
        if (user == null || (passwordHash != user.PasswordValidation))
        {

            return new UserError.InvalidCredentials();
        }

        // 3. Gerir limite de tokens
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

        // 4. Gerar o novo Token usando o SERVIÇO DE TOKENS
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
                if (user == null)
                {
                    return new UserError.FailedToGetUserInfo();
                }
                return user;
        });
    }
    public async Task<OneOf<bool, UserError>> EditUser(int userId, string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername)) return new UserError.InvalidName();

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
            if (!_usersDomain.IsSafePassword(newPassword))
            {
                return new UserError.InvalidPassword();
            }

            return await _transactionManager.Run<OneOf<string, UserError>>(async (context) =>
            {
                var user = await context.UserRepository.GetUserById(userId);
                if (user == null)
                {
                    return new UserError.UserNotFound();
                }

                var oldPasswordHash = _passwordEncoder.createValidationInformation(oldPassword);
                Console.Write("oldPassordHash:");
                Console.Write(oldPasswordHash);
                if (user.PasswordValidation != oldPasswordHash)
                {
                    return new UserError.OldPasswordIsIncorrect();
                }
                var newPasswordHash = _passwordEncoder.createValidationInformation(newPassword);
                Console.Write("newPasswordHash:");
                Console.Write(newPasswordHash);
                if (user.PasswordValidation == newPasswordHash)
                {
                    return new UserError.NewPasswordCannotBeTheSameAsTheOldPassword();
                }

                await context.UserRepository.ChangePassword(userId, newPasswordHash);
                return "Password changed successfully";
            });
        }

     
}
}