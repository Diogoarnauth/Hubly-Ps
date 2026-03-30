using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ISocialPlatformRepository
    {
        Task<bool> Exists(int platformId);
        Task<List<SocialPlatform>> GetAll();    
    
    }

}