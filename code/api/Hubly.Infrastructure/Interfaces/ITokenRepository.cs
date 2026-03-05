using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ITokenRepository
    {
        Task<Token?> GetTokenByValidation(string tokenValidation);

        Task<Token?> GetTokenByUserId(int userId);

        Task<bool> CreateToken(Token token);

        Task<bool> DeleteToken(int userId, string tokenValidation);

        Task<bool> DeleteTokenByValidation(string tokenValidation);

        Task<bool> RefreshToken(string tokenValidation);
    }
}