using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> CreateUser (User newUser); 
        Task<User?> GetUserByEmail(string email);
        Task<bool> UserExistsWithEmail(string email);    
        Task<User?> GetUserById (int user_id);
        Task EditUser(int userId, string newUsername);
    }

}