namespace Hubly.api.Services.Problems
{
    public abstract class UserError
    {
        public class FailedUserCreation : UserError { }
        public class InvalidName : UserError { }
        public class InvalidEmail : UserError { }
        public class InvalidEmailPasswordCombination : UserError {}
        public class EmailAlreadyExists : UserError { }
        public class InvalidPassword : UserError { }
        public class FailedToGetUserInfo : UserError { } 
        public class InvalidCredentials : UserError { }
        
        
    }
}
