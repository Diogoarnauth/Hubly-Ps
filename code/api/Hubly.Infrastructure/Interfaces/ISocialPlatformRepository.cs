

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ISocialPlatformRepository
    {
        Task<bool> Exists(int platformId);
            
    
    }

}