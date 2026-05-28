using Hubly.api.Services.Fixtures;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace Hubly.api.Services.Tests;

public class ConversationTagServiceTests : IClassFixture<ConversationTagServiceFixture>
{
    private readonly ConversationTagServiceFixture _fixture;

    public ConversationTagServiceTests(ConversationTagServiceFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks();
    }

    [Fact]
    public async Task CreateTag_ShouldReturnId_WhenValid()
    {
        _fixture.SetupUserExists(true);
        _fixture.SetupTagNameExists(false);
        _fixture.ConversationTagRepository.Setup(r => r.CreateTag(It.IsAny<ConversationTag>()))
            .ReturnsAsync(10);

        var result = await _fixture.ConversationTagService.CreateTag(_fixture.UserId, "Nova Tag", "#FFFFFF");

        Assert.True(result.IsT0);
        Assert.Equal(10, result.AsT0);
    }

    [Fact]
    public async Task CreateTag_ShouldReturnError_WhenTagNameAlreadyExists()
    {
        _fixture.SetupUserExists(true);
        _fixture.SetupTagNameExists(true);
        _fixture.SetupCreateTag(10);

        var result = await _fixture.ConversationTagService.CreateTag(_fixture.UserId, "Duplicada", "#FFFFFF");

        Assert.True(result.IsT1);
        Assert.IsType<ConversationTagError.TagNameAlreadyExists>(result.AsT1);
    }

    [Fact]
    public async Task TagConversation_ShouldAllowGlobalTag_WhenUserIsParticipant()
    {
        var globalTag = new ConversationTag { Id = 5, UserId = null, TagName = "Publica" };
        _fixture.SetupUserIsParticipant(true); // Oculta o Moq do ConversationRepository
        _fixture.SetupTagExists(globalTag);
        _fixture.SetupAssignTag();

        var result = await _fixture.ConversationTagService.TagConversation(_fixture.UserId, 1, 5);

        Assert.True(result.IsT0);
        Assert.True(result.AsT0);
    }

    [Fact]
    public async Task UpdateTag_ShouldReturnUnauthorized_WhenTagDoesNotBelongToUser()
    {
        var tag = new ConversationTag { Id = 1, UserId = 999 }; // Diferente do _fixture.UserId (1)
        _fixture.SetupTagExists(tag);

        var result = await _fixture.ConversationTagService.UpdateTag(_fixture.UserId, 1, "Nova", "#FFFFFF");

        Assert.True(result.IsT1);
        Assert.IsType<ConversationTagError.UnauthorizedAccess>(result.AsT1);
    }

    [Fact]
    public async Task DeleteTag_ShouldReturnNotFound_WhenTagDoesNotExist()
    {
        _fixture.SetupTagExists(null);

        var result = await _fixture.ConversationTagService.DeleteTag(_fixture.UserId, 99);

        Assert.True(result.IsT1);
        Assert.IsType<ConversationTagError.TagNotFound>(result.AsT1);
    }

    [Theory]
    [InlineData("")]          // string.Empty
    [InlineData(null)]        // null
    [InlineData("   ")]       // espaços em branco
    public async Task CreateTag_ShouldReturnError_WhenTagNameIsEmpty(string invalidName)
    {
        _fixture.SetupUserExists(true);

        var result = await _fixture.ConversationTagService.CreateTag(_fixture.UserId, invalidName, "#FFFFFF");

        Assert.True(result.IsT1);
        Assert.IsType<ConversationTagError.InvalidTagName>(result.AsT1);
    }

    [Fact]
    public async Task CreateTag_ShouldReturnError_WhenTagNameIsTooLong()
    {
        _fixture.SetupUserExists(true);
        var longName = new string('A', 51);

        var result = await _fixture.ConversationTagService.CreateTag(_fixture.UserId, longName, "#FFFFFF");

        Assert.True(result.IsT1);
        Assert.IsType<ConversationTagError.InvalidTagName>(result.AsT1);
    }

    [Fact]
    public async Task UpdateTag_ShouldReturnError_WhenNewNameIsTakenByAnotherTag()
    {
        var tagExistente = new ConversationTag { Id = 1, UserId = _fixture.UserId, TagName = "Tag A" };

        _fixture.SetupTagExists(tagExistente);
        _fixture.SetupTagNameExists(true);

        _fixture.ConversationTagRepository
            .Setup(r => r.GetById(1))
            .ReturnsAsync(new ConversationTag { Id = 2, TagName = "Tag B" });

        var result = await _fixture.ConversationTagService.UpdateTag(_fixture.UserId, 1, "Tag B", "#FFFFFF");

        Assert.True(result.IsT1);
        Assert.IsType<ConversationTagError.UnauthorizedAccess>(result.AsT1);
    }

    [Fact]
    public async Task UpdateTag_ShouldSucceed_WhenUpdatingTagWithSameName()
    {
        var tag = new ConversationTag { Id = 1, UserId = _fixture.UserId, TagName = "Tag A" };

        _fixture.SetupTagExists(tag);
        _fixture.SetupTagNameExists(true);

        var result = await _fixture.ConversationTagService.UpdateTag(_fixture.UserId, 1, "Tag A", "#FFFFFF");

        Assert.True(result.IsT0);
        Assert.True(result.AsT0);
    }

    [Fact]
    public async Task TagConversation_ShouldReturnError_WhenUserIsNotParticipant()
    {
        _fixture.ConversationRepository
            .Setup(r => r.IsUserParticipant(1, _fixture.UserId))
            .ReturnsAsync(false);

        var result = await _fixture.ConversationTagService.TagConversation(_fixture.UserId, 1, 5);

        Assert.True(result.IsT1);
        Assert.IsType<ConversationTagError.UnauthorizedAccess>(result.AsT1);
    }

    [Fact]
    public async Task TagConversation_ShouldReturnError_WhenTagDoesNotBelongToUser()
    {
        _fixture.SetupUserIsParticipant(true);

        var otherUserTag = new ConversationTag { Id = 10, UserId = 99, TagName = "Privada" };
        _fixture.SetupTagExists(otherUserTag);

        var result = await _fixture.ConversationTagService.TagConversation(_fixture.UserId, 1, 10);

        Assert.True(result.IsT1);
        Assert.IsType<ConversationTagError.UnauthorizedAccess>(result.AsT1);
    }

    [Fact]
    public async Task GetUserTags_ShouldReturnEmptyList_WhenUserHasNoTags()
    {
        _fixture.SetupUserExists(true);
        _fixture.SetupGetUserTags(new List<ConversationTag>()); 

        var result = await _fixture.ConversationTagService.GetUserTags(_fixture.UserId);

        Assert.True(result.IsT0);
        Assert.Empty(result.AsT0);
    }

    [Fact]
    public async Task UntagConversation_ShouldSucceed_WhenRemovingTag()
    {
        _fixture.SetupUserIsParticipant(true);
        _fixture.SetupRemoveTag();

        var result = await _fixture.ConversationTagService.UntagConversation(_fixture.UserId, 1);

        Assert.True(result.IsT0);
        Assert.True(result.AsT0);

        _fixture.ConversationTagRepository.Verify(r => r.RemoveTag(_fixture.UserId, 1), Times.Once);
    }
}

