
using System.Security.Cryptography;
using BCrypt.Net;

namespace Hubly.api.Domain.Entities;

public class CompaniesDomain
{
    private readonly CompaniesDomainConfig _config;
    private readonly ProfanityFilter.ProfanityFilter _profanityFilter;


    public CompaniesDomain(CompaniesDomainConfig config)
    {
        _config = config;
        _profanityFilter = new ProfanityFilter.ProfanityFilter();

    }

    public bool IsValidSector(string sector)
    {
        if (_profanityFilter.IsProfanity(sector))
        {
            return false;
        }
        return true;
    }

}
