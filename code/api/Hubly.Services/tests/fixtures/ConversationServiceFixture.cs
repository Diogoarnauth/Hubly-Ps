using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Services;
using Hubly.api.Services.Interfaces;
using Moq;
using Microsoft.Extensions.Configuration;
using Hubly.api.Services.Problems;
using OneOf;


namespace Hubly.api.Services.Fixtures
{
    public class ConversationServiceFixture
    {
        public readonly Mock<IConversationRepository> ConversationRepository;
        public readonly Mock<IMessageRepository> MessageRepository;
        public readonly Mock<ICompanyRepository> CompanyRepository;
        public readonly Mock<ICreatorSocialRepository> CreatorSocialRepository;
        public readonly Mock<ITransactionManager> TransactionManager;
        public readonly Mock<ITransactionContext> TransactionContext;
        public readonly Mock<IEventService> EventService;


        public readonly ConversationService ConversationService;

        public readonly int UserId = 1;
        public readonly int OtherUserId = 2;
        public readonly Conversation TestConversation;

        public ConversationServiceFixture()
        {
            ConversationRepository = new Mock<IConversationRepository>();
            MessageRepository = new Mock<IMessageRepository>();
            CompanyRepository = new Mock<ICompanyRepository>();
            CreatorSocialRepository = new Mock<ICreatorSocialRepository>();
            TransactionManager = new Mock<ITransactionManager>();
            TransactionContext = new Mock<ITransactionContext>();
            EventService = new Mock<IEventService>();

            SetupTransactionContext();

            TestConversation = new Conversation
            {
                Id = 1,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                LastMessageAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            SetupTransactionManager();

            ConversationService = new ConversationService(
                TransactionManager.Object,
                EventService.Object
            );

            SetupDefaultMocks();
        }

        private void SetupTransactionContext()
        {
            TransactionContext.Setup(tc => tc.ConversationRepository).Returns(ConversationRepository.Object);
            TransactionContext.Setup(tc => tc.MessageRepository).Returns(MessageRepository.Object);
            TransactionContext.Setup(tc => tc.CompanyRepository).Returns(CompanyRepository.Object);
            TransactionContext.Setup(tc => tc.CreatorSocialRepository).Returns(CreatorSocialRepository.Object);
        }

        private void SetupTransactionManager()
        {
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<int, ConversationError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<int, ConversationError>>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<bool, ConversationError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<bool, ConversationError>>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<PagedResponse<Message>, ConversationError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<PagedResponse<Message>, ConversationError>>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<List<ConversationWithLastMessage>, ConversationError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<List<ConversationWithLastMessage>, ConversationError>>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<ConversationService.SendMessageResult, ConversationError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<ConversationService.SendMessageResult, ConversationError>>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<(int ConversationId, List<int> ParticipantProfileIds), ConversationError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<(int ConversationId, List<int> ParticipantProfileIds), ConversationError>>>>(
                    async func => await func(TransactionContext.Object));

        }

        public void ResetMocks()
        {
            ConversationRepository.Reset();
            MessageRepository.Reset();
            CompanyRepository.Reset();
            CreatorSocialRepository.Reset();
            EventService.Reset();
            TransactionManager.Reset();
            TransactionContext.Reset();

            SetupTransactionContext();

            SetupTransactionManager();
            SetupDefaultMocks();
        }

        private void SetupDefaultMocks()
        {
            ConversationRepository
                .Setup(repo => repo.IsUserParticipant(TestConversation.Id, UserId))
                .ReturnsAsync(true);

            ConversationRepository
                .Setup(repo => repo.GetById(TestConversation.Id))
                .ReturnsAsync(TestConversation);
        }

        public void SetupMessageAdd(int messageId)
        {
            MessageRepository
                .Setup(repo => repo.AddMessage(It.IsAny<Message>()))
                .ReturnsAsync(messageId);
        }


        public void SetupConversationExists(Conversation conversation)
        {
            ConversationRepository
                .Setup(repo => repo.GetById(conversation.Id))
                .ReturnsAsync(conversation);

            ConversationRepository
                .Setup(repo => repo.GetConversationWithParticipants(conversation.Id))
                .ReturnsAsync(conversation);
        }

        public void SetupParticipantStatus(int conversationId, int userId, bool isParticipant)
        {
            ConversationRepository
                .Setup(repo => repo.IsUserParticipant(conversationId, userId))
                .ReturnsAsync(isParticipant);
        }

        public void SetupGetMessages(int conversationId, PagedResponse<Message> response)
        {
            MessageRepository
                .Setup(repo => repo.GetMessages(conversationId, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(response);
        }

        public void SetupCompanyRepository(int companyId, Company company)
        {
            CompanyRepository.Setup(r => r.GetByUserId(companyId)).ReturnsAsync(company);
        }

        public void SetupCreatorSocialRepository(int socialId, CreatorSocialProfile profile)
        {
            CreatorSocialRepository.Setup(r => r.GetById(socialId)).ReturnsAsync(profile);
        }

        public void SetupMessageById(int messageId, Message message)
        {
            MessageRepository.Setup(r => r.GetById(messageId)).ReturnsAsync(message);
        }

        public void SetupCreatorSocial(int socialId, CreatorSocialProfile profile)
        {
            CreatorSocialRepository.Setup(r => r.GetById(socialId)).ReturnsAsync(profile);
        }

        public void SetupCompany(int companyId, Company company)
        {
            CompanyRepository.Setup(r => r.GetByUserId(companyId)).ReturnsAsync(company);
        }

    }
}