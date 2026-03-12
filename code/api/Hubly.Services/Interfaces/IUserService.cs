using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface IUserService
    {
        Task<OneOf<User, UserError>> Register(string email, string password, string username);
        Task<OneOf<string,UserError>> Token(string email, string password);
        Task<OneOf<string, UserError>> Logout(string tokenValue);
        Task<OneOf<bool, UserError>> EditUser(int userId, string newUsername);
        Task<OneOf<User, UserError>>  GetUserInfo(int userId);
        Task<OneOf<string, UserError>> ChangePassword(int userId, string oldPassword, string newPassword);
        

    }

}   
