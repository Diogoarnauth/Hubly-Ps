using System.Text.RegularExpressions; 
using System.Globalization;
using System.Linq;
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

    public string ConvertCompanySize(int size)
{
    if (size < 0) return "0 a 100"; 

    return size switch
    {
        <= 100 => "0 a 100",
        <= 1000 => "100 a 1000",
        <= 10000 => "1000 a 10000",
        <= 100000 => "10000 a 100000",
        <= 1000000 => "100000 a 1000000",
        _ => "+1M"
    };
}

    public bool IsSafeText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false; 
        return !_profanityFilter.IsProfanity(text);
    }

    public bool IsValidWebsite(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;

        if (_profanityFilter.IsProfanity(url)) return false;

        string pattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
        return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
    }

    public bool IsValidCountry(string? countryName)
    {
        if (string.IsNullOrEmpty(countryName)) return true;

        try 
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(x => new RegionInfo(x.Name))
                .Any(x => x.EnglishName.Equals(countryName, StringComparison.OrdinalIgnoreCase) || 
                          x.DisplayName.Equals(countryName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

}
