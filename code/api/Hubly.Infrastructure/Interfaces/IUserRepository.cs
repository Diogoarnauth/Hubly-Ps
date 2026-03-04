using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> CreateUser (User newUser); 

        Task<bool> UserExistsWithEmail(string email);
        
    }

}