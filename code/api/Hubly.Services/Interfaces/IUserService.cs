using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface IUserService
    {
        Task<OneOf<User, UserError>> Register(string email, string password, string username);
        Task<OneOf<string, UserError>> Token(string email, string password);
        Task<OneOf<string, UserError>> Logout(string tokenValue);
        Task<OneOf<bool, UserError>> EditUser(int userId, string newUsername);
        Task<OneOf<User, UserError>> GetUserInfo(int userId);
        Task<OneOf<bool, UserError>> CheckCreatorOrCompany(int userId);
        Task<OneOf<string, UserError>> ChangePassword(int userId, string oldPassword, string newPassword);
        Task<OneOf<string, UserError>> ResendEmailConfirmation(string email);
        Task<OneOf<string, UserError>> GenerateConfirmationCode(int userId, ITransactionContext context);
        Task<OneOf<bool, UserError>> VerifyConfirmationCodeAsync(string email, string code);
        Task<OneOf<bool, UserError>> ResendConfirmationCodeAsync(int userId);
        Task<OneOf<PagedResponse<ProfileViewHistory>, UserError>> GetHistory(int userId, int page = 1, int pageSize = 20);
        Task<OneOf<User, UserError>> GetFullCreatorProfile(int targetCreatorId, int viewerId);
        Task<OneOf<User, UserError>> GetFullCompanyProfile(int targetCompanyId, int viewerId);

    }
}
