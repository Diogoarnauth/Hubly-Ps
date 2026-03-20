using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Hubly.api.Domain.Entities;

public class CreatorsDomain
{
    private readonly CreatorsDomainConfig _config;
    private readonly ProfanityFilter.ProfanityFilter _profanityFilter;

    public const string StatusAvailable = "AVAILABLE";
    public const string StatusUnavailable = "UNAVAILABLE";

    public CreatorsDomain(CreatorsDomainConfig config)
    {
        _config = config;
        _profanityFilter = new ProfanityFilter.ProfanityFilter();
    }

    public bool IsValidArtisticName(string artisticName)
    {
        if (string.IsNullOrWhiteSpace(artisticName))
            return false;

        if (artisticName.Length < _config.MinArtitisticNameLength)
            return false;

        if (_profanityFilter.IsProfanity(artisticName))
        {
            return false;
        }

        return true;
    }

    public bool IsValidAvailabilityStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return false;

        return status.Equals(StatusAvailable, StringComparison.OrdinalIgnoreCase) ||
               status.Equals(StatusUnavailable, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsValidRating(int rate)
    {
        if (rate >= 0 && rate <= 5)
        {
            return true;
        }

        return false;
    }



    public (decimal NewGlobalRating, int NewRatingsCount) CalculateNewRating(decimal currentRating, int currentCount, int newRate)
    {
        int newCount = currentCount + 1;

        decimal newGlobalRating = ((currentRating * currentCount) + newRate) / newCount;

        return (newGlobalRating, newCount);
    }
}