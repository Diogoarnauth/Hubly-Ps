namespace Hubly.api.Domain.Entities;

public class CreatorInterestProfile
{
    public Dictionary<int, int> SectorFrequencies { get; set; }

    public Dictionary<int, int> PlatformFrequencies { get; set; }

    public double AveragePriceViewed { get; set; }

    public CreatorInterestProfile()
    {
        SectorFrequencies = new Dictionary<int, int>();
        PlatformFrequencies = new Dictionary<int, int>();
        AveragePriceViewed = 0;
    }

    public CreatorInterestProfile(
        Dictionary<int, int> sectorFrequencies, 
        Dictionary<int, int> platformFrequencies, 
        double averagePriceViewed)
    {
        SectorFrequencies = sectorFrequencies;
        PlatformFrequencies = platformFrequencies;
        AveragePriceViewed = averagePriceViewed;
    }
}