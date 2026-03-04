using Hubly.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> CreateUser (User newUser); 
    }

}