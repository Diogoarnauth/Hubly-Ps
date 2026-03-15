namespace Hubly.api.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string email, string username, string confirmationCode);
    }
}
