
using System.Security.Cryptography;
using BCrypt.Net;

namespace Hubly.api.Domain.Entities;

public class UsersDomain
{
    private readonly UsersDomainConfig _config;
    private readonly ProfanityFilter.ProfanityFilter _profanityFilter;


    public UsersDomain(UsersDomainConfig config)
    {
        _config = config;
        _profanityFilter = new ProfanityFilter.ProfanityFilter();

    }

    public int MaxTokensPerUser => _config.MaxTokensPerUser;

    public bool IsSafePassword(string password)
    {
        if (password.Length < _config.MinPasswordLength) return false;
        
        return password.Any(char.IsDigit) && password.Any(char.IsUpper);
    }

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

         if (_profanityFilter.IsProfanity(username))
        {
            return false;
        }


        return true;
    }
        
}
