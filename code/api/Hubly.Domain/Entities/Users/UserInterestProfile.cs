namespace Hubly.api.Domain.Entities;

public record UserInterestProfile(
    Dictionary<int, int> SectorFrequencies, 
    Dictionary<string, int> CountryFrequencies, 
    Dictionary<string, int> SizeFrequencies 
);