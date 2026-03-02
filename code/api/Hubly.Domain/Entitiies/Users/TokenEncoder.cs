namespace Hubly.Domain.Entities;

public interface TokenEncoder
{
    TokenValidationInfo CreateValidationInformation(string token);
}