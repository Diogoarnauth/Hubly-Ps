using System.Security.Cryptography;
using BCrypt.Net;

namespace Hubly.api.Domain.Entities;

public class CreatorsDomain
{
    private readonly CreatorsDomainConfig _config;

    public const string StatusAvailable = "AVAILABLE";
    public const string StatusUnavailable = "UNAVAILABLE";

    public CreatorsDomain(CreatorsDomainConfig config)
    {
        _config = config;
    }

    public bool IsValidArtisticName(string artisticName)
    {
        if (string.IsNullOrWhiteSpace(artisticName)) 
            return false;

        if (artisticName.Length < _config.MinArtitisticNameLength) 
            return false;

        return true;
    }

    public bool IsValidAvailabilityStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status)) 
            return false;

        return status.Equals(StatusAvailable, StringComparison.OrdinalIgnoreCase) || 
               status.Equals(StatusUnavailable, StringComparison.OrdinalIgnoreCase);
    }
        
}
