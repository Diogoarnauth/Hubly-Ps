namespace Hubly.api.Services.Interfaces
{
    public interface ITokenEncoder
    {
        string CreateValidationInformation(string token);
    }
}
