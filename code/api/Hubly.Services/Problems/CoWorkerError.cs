using System.Diagnostics.Contracts;

namespace Hubly.api.Services.Problems
{
    public abstract class CoWorkerError
    {
        public class UserNotFound : CoWorkerError { }
        public class AlreadyInvited : CoWorkerError { }
        public class InviteNotFound : CoWorkerError { }
        public class InviteExpired : CoWorkerError { }
        public class Unauthorized : CoWorkerError { }
        public class CannotInviteSelf : CoWorkerError { }
        public class UserCannotBeACoWorker : CoWorkerError { }
        public class UserAlreadyACoWorker : CoWorkerError { }
        public class UserIsNotACreatorOrCompany : CoWorkerError { }
        
    }
}
