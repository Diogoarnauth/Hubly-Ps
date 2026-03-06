using System.Security.Cryptography;
using System.Text;
using Hubly.api.Domain.Entities;

namespace Hubly.api.Domain.Entities;
/*
public class Sha256TokenEncoder : TokenEncoder
{
    public TokenValidationInfo CreateValidationInformation(string token)
    {
        return new TokenValidationInfo(Hash(token));
    }

    private string Hash(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        
        byte[] hashBytes = SHA256.HashData(inputBytes);
        
        
        return Convert.ToBase64String(hashBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('='); 
    }
}*/