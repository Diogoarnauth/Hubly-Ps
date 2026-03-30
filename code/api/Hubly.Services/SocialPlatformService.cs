using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;
using System.Data.Common;
using System.Linq;
using Microsoft.Extensions.Configuration;


namespace Hubly.api.Services
{
    public class SocialPlatformService : ISocialPlatformService
    {
        private readonly ITransactionManager _transactionManager;

        public SocialPlatformService(
            ITransactionManager transactionManager,
            IConfiguration configuration
        )
        {
            _transactionManager = transactionManager;
        }


        public async Task<OneOf<List<SocialPlatform>, SocialPlatformError>> GetAllPlatforms()
        {
            return await _transactionManager.Run<OneOf<List<SocialPlatform>, SocialPlatformError>>(async (context) =>
            {
                var platforms = await context.SocialPlatformRepository.GetAll();

                if (platforms == null) return new SocialPlatformError.FailedToGetPlatforms();
                
                return platforms.ToList();
            });
        }
    }
}
