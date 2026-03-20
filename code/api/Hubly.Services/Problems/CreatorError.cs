using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Hubly.api.Services.Problems
{
    public abstract class CreatorError
    {
        public class FailedCreatorCreation : CreatorError { }
        public class InvalidArtisticName : CreatorError { }
        public class CreatorAlreadyExists : CreatorError { }
        public class FailedToGetCreatorInfo : CreatorError { } 
        public class FailedToEditCreator : CreatorError { }   
        public class CreatorNotFound : CreatorError { }
        public class UserAlreadyRegisteredAsCompany : CreatorError { }
        public class InvalidAvailabilityStatus: CreatorError{ }
        public class FailedToUpdateStatus: CreatorError { }
        public class InvalidRating: CreatorError{ }
        public class ErrorRatingCreator : CreatorError { }

    }
}
