using System.Diagnostics.Contracts;

namespace Hubly.api.Services.Problems
{
    public abstract class CompanyError
    {
        //public class FailedCreatorCreation : CompanyError { }
        public class InvalidArtisticName : CompanyError { }
        public class CompanyAlreadyExists : CompanyError { }
        public class InvalidSectorName : CompanyError { }
        public class FailedToGetCompanyInfo : CompanyError { } 
        //public class FailedToEditCreator : CompanyError { }   
        //public class CreatorNotFound : CompanyError { }
        public class UserAlreadyRegisteredAsCreator : CompanyError { }
        
    }
}
