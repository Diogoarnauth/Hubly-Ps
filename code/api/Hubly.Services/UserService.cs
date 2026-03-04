using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.Domain.Entities;
using OneOf;


namespace Hubly.api.Services
{
    public class UserService: IUserService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly UsersDomain _usersDomain;
    }

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
            
            return await _transactionManager
     
        }

        
}