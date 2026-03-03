using Hubly.Domain.Entities;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface IUserInterface
    {

        Task<OneOf<User, UserError>> Register(string email, string password, string username);
        

    }

}   
