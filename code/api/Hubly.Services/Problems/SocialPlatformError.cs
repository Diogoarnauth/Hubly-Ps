using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Hubly.api.Domain.Entities;

namespace Hubly.api.Services.Problems
{
    public abstract class SocialPlatformError
    {
        public class FailedToGetPlatforms: SocialPlatformError { }

    }
}
