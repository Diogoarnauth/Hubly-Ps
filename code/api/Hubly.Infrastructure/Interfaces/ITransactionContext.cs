namespace Hubly.api.Infrastructure.Interfaces;

public interface ITransactionContext
{
        IUserRepository UserRepository { get; }
        ITokenRepository TokenRepository {get; }
        IEmailConfirmationRepository EmailConfirmationRepository {get; }
        ICreatorRepository CreatorRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IHistoryRepository HistoryRepository { get; }
        ISocialPlatformRepository SocialPlatformRepository { get; }
        ICreatorSocialRepository CreatorSocialRepository { get; }
        IConversationRepository ConversationRepository { get; }
        IMessageRepository MessageRepository { get; }
        IConversationTagRepository ConversationTagRepository { get; }
        ICoWorkerRepository CoWorkerRepository { get; }

}