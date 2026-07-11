using Hubly.api.Services.Fixtures;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace Hubly.api.Services.Tests;

public class ConversationServiceTests : IClassFixture<ConversationServiceFixture>
{
    private readonly ConversationServiceFixture _fixture;

    public ConversationServiceTests(ConversationServiceFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks();
    }



    [Fact]
    public async Task SendMessage_ShouldReturnMessageId_WhenValid()
    {

        var messageId = 100;
        _fixture.SetupMessageAdd(messageId);
        _fixture.SetupConversationExists(_fixture.TestConversation);

        var result = await _fixture.ConversationService.SendMessage(_fixture.UserId, null, _fixture.TestConversation.Id, "Olá!");

        Assert.True(result.IsT0);
        Assert.Equal(messageId, result.AsT0);
        _fixture.EventService.Verify(e => e.SendToTopic(It.IsAny<string>(), "NewMessage", It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task SendMessage_ShouldReturnAccessDenied_WhenUserIsNotParticipant()
    {
        _fixture.SetupParticipantStatus(_fixture.TestConversation.Id, _fixture.UserId, false);

        var result = await _fixture.ConversationService.SendMessage(_fixture.UserId, null, _fixture.TestConversation.Id, "Olá!");

        Assert.True(result.IsT1);
        Assert.IsType<ConversationError.AccessDenied>(result.AsT1);
    }

    [Fact]
    public async Task GetMessages_ShouldReturnMessages_WhenUserIsParticipant()
    {
        var pagedMessages = new PagedResponse<Message> { Items = new List<Message> { new Message { Id = 1 } } };
        _fixture.SetupGetMessages(_fixture.TestConversation.Id, pagedMessages);

        var result = await _fixture.ConversationService.GetMessages(_fixture.UserId, _fixture.TestConversation.Id);

        Assert.True(result.IsT0);
        Assert.Single(result.AsT0.Items);
    }

    [Fact]
    public async Task CreateConversation_ShouldReturnId_WhenValid()
    {

        var company = new Company { Id = _fixture.UserId };
        _fixture.SetupCompanyRepository(1, company); // Sender company
        _fixture.SetupCompanyRepository(2, new Company { Id = _fixture.OtherUserId }); // Receiver company

        _fixture.ConversationRepository
            .Setup(r => r.GetConversationByParticipants(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync((Conversation)null);

        _fixture.ConversationRepository
            .Setup(r => r.AddConversation(It.IsAny<Conversation>()))
            .ReturnsAsync(99);

        var result = await _fixture.ConversationService.CreateConversation(_fixture.UserId, null, 1, null, 2, null);

        Assert.True(result.IsT0);
        Assert.Equal(99, result.AsT0);
    }

    [Fact]
    public async Task DeleteMessage_ShouldReturnAccessDenied_WhenUserIsNotOwner()
    {
        var message = new Message { Id = 1, SenderId = 999 };
        _fixture.SetupMessageById(1, message);

        var result = await _fixture.ConversationService.DeleteMessage(_fixture.UserId, null, 1);

        Assert.True(result.IsT1);
        Assert.IsType<ConversationError.AccessDenied>(result.AsT1);
    }

    [Fact]
    public async Task MarkMessagesAsRead_ShouldReturnAccessDenied_WhenUserIsNotParticipant()
    {
        var conversation = new Conversation { Id = 1 };
        _fixture.ConversationRepository
            .Setup(r => r.GetConversationWithParticipants(1))
            .ReturnsAsync(conversation);

        var result = await _fixture.ConversationService.MarkMessagesAsRead(_fixture.UserId, 1, 100);

        Assert.True(result.IsT1);
        Assert.IsType<ConversationError.AccessDenied>(result.AsT1);
    }

    [Fact]
    public async Task GetCreatorConversationsByProfile_ShouldReturnConversations_WhenUserIsOwner()
    {
        var socialId = 5;
        var profile = new CreatorSocialProfile { Id = socialId, CreatorId = _fixture.UserId };
        _fixture.SetupCreatorSocial(socialId, profile);

        var conversations = new List<ConversationWithLastMessage> { new() };
        _fixture.ConversationRepository
            .Setup(r => r.GetCreatorConversationsByProfileExtended(_fixture.UserId, socialId))
            .ReturnsAsync(conversations);

        var result = await _fixture.ConversationService.GetCreatorConversationsByProfile(_fixture.UserId, socialId);

        Assert.True(result.IsT0);
        Assert.Equal(conversations, result.AsT0);
    }

    [Fact]
    public async Task GetCreatorConversationsByProfile_ShouldReturnAccessDenied_WhenUserIsNotOwner()
    {
        var socialId = 5;
        var profile = new CreatorSocialProfile { Id = socialId, CreatorId = _fixture.OtherUserId };
        _fixture.SetupCreatorSocial(socialId, profile);

        var result = await _fixture.ConversationService.GetCreatorConversationsByProfile(_fixture.UserId, socialId);

        Assert.True(result.IsT1);
        Assert.IsType<ConversationError.AccessDenied>(result.AsT1);
    }

    [Fact]
    public async Task GetCompanyConversations_ShouldReturnConversations_WhenValid()
    {
        
        var companyId = 10;
        var company = new Company { Id = companyId }; 
        _fixture.SetupCompany(companyId, company);

        _fixture.ConversationRepository
            .Setup(r => r.GetCompanyConversationsExtended(companyId, companyId))
            .ReturnsAsync(new List<ConversationWithLastMessage>());

        var result = await _fixture.ConversationService.GetCompanyConversations(companyId, companyId);

        Assert.True(result.IsT0);
    }
}


