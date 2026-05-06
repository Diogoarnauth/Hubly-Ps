namespace Hubly.api.Services.Problems;

public abstract class ConversationTagError
{
    public class TagNotFound : ConversationTagError { }
    public class UserNotFound : ConversationTagError { }
    public class UnauthorizedAccess : ConversationTagError { }
    public class TagNameAlreadyExists : ConversationTagError { }
    public class InvalidTagName : ConversationTagError { }
    public class ConversationNotFound : ConversationTagError { }
    public class InvalidColorHex : ConversationTagError { }
}
