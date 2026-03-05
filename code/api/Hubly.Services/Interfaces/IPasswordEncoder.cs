namespace Hubly.api.Services.Interfaces
{
    public interface IPasswordEncoder
    {
        string createValidationInformation(string password);
    }
}
