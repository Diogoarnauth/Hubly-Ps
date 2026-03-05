using Hubly.api.Infrastructure.Interfaces;

namespace  Hubly.api.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateTokenValue();
        string CreateTokenValidationInformation(string token);
        Task<bool> CreateToken(int userId, string validationInformation, ITransactionContext context);
        Task<int?> ValidateToken(string token, ITransactionContext? context);
        Task<bool> DeleteToken(string tokenValue, ITransactionContext? context);
    }
}
