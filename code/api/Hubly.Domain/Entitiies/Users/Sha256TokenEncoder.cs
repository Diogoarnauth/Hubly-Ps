using System.Security.Cryptography;
using System.Text;
using Hubly.api.Domain.Entities;

namespace Hubly.api.Domain.Entities;

public class Sha256TokenEncoder : TokenEncoder
{
    public TokenValidationInfo CreateValidationInformation(string token)
    {
        return new TokenValidationInfo(Hash(token));
    }

    private string Hash(string input)
    {
        // 1. Converte a string para bytes
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        
        // 2. Calcula o Hash SHA256
        byte[] hashBytes = SHA256.HashData(inputBytes);
        
        // 3. Converte para Base64 "URL Safe" (igual ao que o Kotlin faz)
        // Substituímos caracteres que podem dar erro em URLs (+ e /)
        return Convert.ToBase64String(hashBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('='); // Remove o padding '=' que não é necessário em URLs
    }
}