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
        private readonly ITransactionManager _transactionManager;
        private readonly UsersDomain _usersDomain;
    

    public UserService(
        ITokenService tokenService,
        ITransactionManager transactionManager,
        UsersDomain usersDomain
    )
    {
        _tokenService = tokenService;
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

        var passwordInfo = _usersDomain.CreatePasswordValidationInformation(password);
        
        var newUser = new User {
            Name = userName, 
            Email = email,
            PasswordValidation = passwordInfo.ValidationInfo,
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
        
        if (user == null || !_usersDomain.ValidatePassword(password, user.GetValidationInfo()))
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
  
     
}
}