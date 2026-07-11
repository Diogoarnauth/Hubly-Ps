using System.Diagnostics.Contracts;

namespace Hubly.api.Services.Problems
{
    public abstract class CompanyError
    {
        public class InvalidArtisticName : CompanyError { }
        public class InvalidSectorName : CompanyError { }
        public class CompanyAlreadyExists : CompanyError { }
        public class FailedToGetCompanyInfo : CompanyError { } 
        public class CompanyNotFound : CompanyError { }
        public class UserAlreadyRegisteredAsCreator : CompanyError { }
        public class InvalidWebSiteLink : CompanyError { }
        public class InvalidCountryHeadquarters : CompanyError { }
        public class UserAlreadyRegisteredAsCoWorker : CompanyError { }

        
    }
}
