using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface IEmailConfirmationRepository
    {
        Task<EmailConfirmation> CreateConfirmationCodeAsync(int userId, string confirmationCode, int expiryHours = 24);
        Task<EmailConfirmation> GetConfirmationCodeAsync(string confirmationCode);
        Task<bool> MarkConfirmationCodeAsUsedAsync(int confirmationCodeId);
        Task<bool> ConfirmUserEmailAsync(int userId);
        Task<bool> CodeExists(int userId);
        Task<List<EmailConfirmation>> GetExpiredConfirmationCodes();
        Task DeleteConfirmationCodes(List<EmailConfirmation> confirmationCodes);
        Task DeleteConfirmationCodesById(List<int> codeIds);


    }
}
