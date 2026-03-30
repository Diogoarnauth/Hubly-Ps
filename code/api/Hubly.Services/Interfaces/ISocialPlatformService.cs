using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface ISocialPlatformService
    {
        Task<OneOf<List<SocialPlatform>, SocialPlatformError>> GetAllPlatforms();
    }

}   