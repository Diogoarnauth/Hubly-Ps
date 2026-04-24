using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure;

public class TransactionContext : ITransactionContext
{
    private readonly HublyDbContext _context;

    public TransactionContext (HublyDbContext context)
    {
        _context = context;
        UserRepository = new UserRepository(context);
        TokenRepository = new TokenRepository(context);
        EmailConfirmationRepository = new EmailConfirmationRepository(context);
        CreatorRepository = new CreatorRepository(context); 
        CompanyRepository = new CompanyRepository(context); 
        HistoryRepository = new HistoryRepository(context);
        SocialPlatformRepository = new SocialPlatformRepository(context);
        CreatorSocialRepository = new CreatorSocialRepository(context);
        ConversationRepository = new ConversationRepository(context);
        MessageRepository = new MessageRepository(context);

    }

    public IUserRepository UserRepository { get;}
    public ITokenRepository TokenRepository {get;}
    public IEmailConfirmationRepository EmailConfirmationRepository {get;}
    public ICreatorRepository CreatorRepository { get; } 
    public ICompanyRepository CompanyRepository { get; } 
    public IHistoryRepository HistoryRepository { get; } 
    public IConversationRepository ConversationRepository { get; }
    public IMessageRepository MessageRepository {get; }
    public ISocialPlatformRepository SocialPlatformRepository { get; }
    public ICreatorSocialRepository CreatorSocialRepository { get; }

}