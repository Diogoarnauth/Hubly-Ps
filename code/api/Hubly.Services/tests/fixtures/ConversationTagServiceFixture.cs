using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Services;
using Hubly.api.Services.Interfaces;
using Moq;
using Microsoft.Extensions.Configuration;
using Hubly.api.Services.Problems;
using OneOf;
using Hubly.api.Infrastructure.Audit;


namespace Hubly.api.Services.Fixtures
{
    public class ConversationTagServiceFixture
    {
        public readonly Mock<IConversationTagRepository> ConversationTagRepository;
        public readonly Mock<IUserRepository> UserRepository;
        public readonly Mock<IConversationRepository> ConversationRepository;
        public readonly Mock<ITransactionManager> TransactionManager;
        public readonly Mock<ITransactionContext> TransactionContext;

        public readonly ConversationTagService ConversationTagService;
        public readonly AuditQueue AuditQueue;


        public readonly int UserId = 1;
        public readonly ConversationTag TestConversationTag;

        public ConversationTagServiceFixture()
        {
            ConversationTagRepository = new Mock<IConversationTagRepository>();
            UserRepository = new Mock<IUserRepository>();
            ConversationRepository = new Mock<IConversationRepository>();
            TransactionManager = new Mock<ITransactionManager>();
            TransactionContext = new Mock<ITransactionContext>();
            AuditQueue = new AuditQueue();


            SetupTransactionContext();

            TestConversationTag = new ConversationTag
            {
                Id = 1,
                UserId = UserId,
                TagName = "Test Tag",
                ColorHex = "#FF0000",
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            SetupTransactionManager();

            ConversationTagService = new ConversationTagService(
                TransactionManager.Object,
                AuditQueue

            );

            SetupDefaultMocks();
        }

        private void SetupTransactionContext()
        {
            TransactionContext.Setup(tc => tc.ConversationTagRepository).Returns(ConversationTagRepository.Object);
            TransactionContext.Setup(tc => tc.UserRepository).Returns(UserRepository.Object);
            TransactionContext.Setup(tc => tc.ConversationRepository).Returns(ConversationRepository.Object);
        }

        private void SetupTransactionManager()
        {
            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<int, ConversationTagError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<int, ConversationTagError>>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<bool, ConversationTagError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<bool, ConversationTagError>>>>(
                    async func => await func(TransactionContext.Object));

            TransactionManager
                .Setup(tm => tm.Run(It.IsAny<Func<ITransactionContext, Task<OneOf<List<ConversationTag>, ConversationTagError>>>>()))
                .Returns<Func<ITransactionContext, Task<OneOf<List<ConversationTag>, ConversationTagError>>>>(
                    async func => await func(TransactionContext.Object));
        }

        public void ResetMocks()
        {
            ConversationTagRepository.Reset();
            UserRepository.Reset();
            ConversationRepository.Reset();
            TransactionManager.Reset();
            TransactionContext.Reset();

            SetupTransactionContext();

            SetupTransactionManager();
            SetupDefaultMocks();
        }


        private void SetupDefaultMocks()
        {
            UserRepository
                .Setup(repo => repo.GetUserById(UserId))
                .ReturnsAsync(new User { Id = UserId });

            ConversationRepository
                .Setup(repo => repo.IsUserParticipant(It.IsAny<int>(), UserId))
                .ReturnsAsync(true);

            ConversationTagRepository
                .Setup(repo => repo.GetById(It.IsAny<int>()))
                .ReturnsAsync(TestConversationTag);

            ConversationTagRepository
                .Setup(repo => repo.TagNameExistsForUser(UserId, It.IsAny<string>()))
                .ReturnsAsync(false);
        }

        public void SetupUserExists(bool exists)
        {
            UserRepository.Setup(r => r.GetUserById(UserId))
                .ReturnsAsync(exists ? new User { Id = UserId } : null);
        }

        public void SetupTagExists(ConversationTag? tag)
        {
            ConversationTagRepository.Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(tag);
        }

        public void SetupTagNameExists(bool exists)
        {
            ConversationTagRepository.Setup(r => r.TagNameExistsForUser(UserId, It.IsAny<string>()))
                .ReturnsAsync(exists);
        }

        public void SetupCreateTag(int newTagId)
        {
            ConversationTagRepository
                .Setup(r => r.CreateTag(It.IsAny<ConversationTag>()))
                .ReturnsAsync(newTagId);
        }

        public void SetupUserIsParticipant(bool isParticipant)
        {
            ConversationRepository
                .Setup(r => r.IsUserParticipant(It.IsAny<int>(), UserId))
                .ReturnsAsync(isParticipant);
        }

        public void SetupAssignTag()
        {
            ConversationTagRepository
                .Setup(r => r.AssignTag(It.IsAny<ConversationTagAssignment>()))
                .Returns(Task.CompletedTask);
        }

        public void SetupDeleteTag(int tagId)
        {
            ConversationTagRepository.Setup(r => r.DeleteTag(tagId)).Returns(Task.CompletedTask);
        }

        public void SetupUpdateTag()
        {
            ConversationTagRepository.Setup(r => r.UpdateTag(It.IsAny<ConversationTag>())).Returns(Task.CompletedTask);
        }

        public void SetupTagNameExistsForUser(bool exists)
        {
            ConversationTagRepository
                .Setup(r => r.TagNameExistsForUser(UserId, It.IsAny<string>()))
                .ReturnsAsync(exists);
        }

        public void SetupGetUserTags(List<ConversationTag> tags)
        {
            ConversationTagRepository.Setup(r => r.GetUserTags(UserId)).ReturnsAsync(tags);
        }

        public void SetupRemoveTag()
        {
            ConversationTagRepository.Setup(r => r.RemoveTag(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.CompletedTask);
        }
    }
}