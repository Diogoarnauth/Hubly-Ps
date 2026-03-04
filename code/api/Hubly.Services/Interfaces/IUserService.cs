using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface IUserService
    {

        Task<OneOf<User, UserError>> Register(string email, string password, string username);
        

    }

}   
