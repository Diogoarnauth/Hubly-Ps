using System.Security.Cryptography;
using BCrypt.Net;

namespace Hubly.api.Domain.Entities;

public class UsersDomain
{
    private readonly UsersDomainConfig _config;

    public UsersDomain(UsersDomainConfig config)
    {
        _config = config;
    }

    public bool IsSafePassword(string password)
    {
        if (password.Length < _config.MinPasswordLength) return false;
        
        return password.Any(char.IsDigit) && password.Any(char.IsUpper);
    }

    // Criar o Hash da password (equivalente ao passwordEncoder.encode)
    public PasswordValidationInfo CreatePasswordValidationInformation(string password)
    {
        string hash = BCrypt.Net.BCrypt.HashPassword(password);
        return new PasswordValidationInfo(hash);
    }

    // Validar se a password bate certo com o hash (equivalente ao matches)
    public bool ValidatePassword(string password, PasswordValidationInfo info)
    {
        return BCrypt.Net.BCrypt.Verify(password, info.ValidationInfo);
    }
/*
    public string GenerateTokenValue()
    {
        byte[] randomNumber = new byte[_config.TokenSizeInBytes];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
*/
    public bool ValidationEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@")) 
        return false;
        
        int atIndex = email.IndexOf("@");
        
        if (atIndex < 1 || atIndex == email.Length - 1)
            return false;

        if (!email.Substring(atIndex).Contains("."))
            return false;

        return true;

    }

    public bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) 
            return false;

        if (username.Length < _config.MinUsernameLength) 
            return false;

        return true;
    }
        
}
