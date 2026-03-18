using System.Diagnostics.Contracts;

namespace Hubly.api.Services.Problems
{
    public abstract class UserError
    {
        public class FailedUserCreation : UserError { }
        public class InvalidName : UserError { }
        public class InvalidEmail : UserError { }
        public class InvalidEmailPasswordCombination : UserError {}
        public class EmailAlreadyExists : UserError { }
        public class EmailAlreadyConfirmed : UserError {}
        public class FailedToLogout : UserError { }
        public class InvalidConfirmationCode : UserError { } 
        public class FailedToConfirmEmail : UserError { }
        public class CodeAlreadyExists : UserError { }  
        public class InvalidPassword : UserError { }
        public class FailedToGetUserInfo : UserError { } 
        public class InvalidCredentials : UserError { }
        public class FailedToEditUser : UserError { }
        public class UserNotFound : UserError { }
        public class OldPasswordIsIncorrect : UserError { }
        public class NewPasswordCannotBeTheSameAsTheOldPassword : UserError { }  
        

        
        
    }
}
