using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;


namespace Hubly.api.Services
{
    public class UserService: IUserService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly UsersDomain _usersDomain;
    

    public UserService(
        ITransactionManager transactionManager,
        UsersDomain usersDomain
    )
    {
        _transactionManager = transactionManager;
        _usersDomain = usersDomain;
    }

    public async Task<OneOf<User,UserError>> Register(string email, string password, string userName)
        {
            if (!_usersDomain.IsSafePassword(password)) return new UserError.InvalidPassword();
            
            if (!_usersDomain.IsValidUsername(userName)) return new UserError.InvalidName();

            if (!_usersDomain.ValidationEmail(email)) return new UserError.InvalidEmail();
            
           return await _transactionManager.Run<OneOf<User, UserError>>(async (context) =>
    {
        if (await context.UserRepository.UserExistsWithEmail(email)) 
            return new UserError.EmailAlreadyExists();

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
     
}
}