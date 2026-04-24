using System.Diagnostics.Contracts;

namespace Hubly.api.Services.Problems
{
    public abstract class ConversationError
    {
        public class InvalidParticipantRole : ConversationError { }
        public class UserNotFound : ConversationError { }
        public class ConversationAlreadyExists : ConversationError { }  
        public class InternalError : ConversationError { }
        public class AccessDenied: ConversationError { }
        public class MessageNotFound: ConversationError { }       
        public class MessageAlreadyDeleted: ConversationError { }       

    }
}
