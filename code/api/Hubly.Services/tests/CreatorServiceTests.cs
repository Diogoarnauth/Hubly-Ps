using Hubly.api.Services.Fixtures;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace Hubly.api.Services.Tests;

public class CreatorServiceTests : IClassFixture<CreatorServiceFixture>
{
    private readonly CreatorServiceFixture _fixture;

    public CreatorServiceTests(CreatorServiceFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks();
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenCreatorAlreadyExists()
    {
        _fixture.SetupCreatorAlreadyExists();

        var result = await _fixture.CreatorService.Register(_fixture.UserId, _fixture.ArtisticName);

        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.CreatorAlreadyExists>(result.AsT1);
    }

    [Fact]
    public async Task Register_ShouldReturnCreator_WhenRegistrationIsSuccessful()
    {
        _fixture.SetupRegisterSuccess();

        var result = await _fixture.CreatorService.Register(_fixture.UserId, _fixture.ArtisticName);

        Assert.True(result.IsT0);
        Assert.Equal(_fixture.UserId, result.AsT0.Id);
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnInvalidStatus_WhenStatusIsUnknown()
    {
        string invalidStatus = "INVALIDO";

        var result = await _fixture.CreatorService.UpdateStatus(_fixture.UserId, null, invalidStatus);

        
        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.InvalidAvailabilityStatus>(result.AsT1);
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnCreatorNotFound_WhenUserDoesNotExist()
    {
        _fixture.SetupCreatorNotFound(_fixture.UserId);

        var result = await _fixture.CreatorService.UpdateStatus(1, null, "AVAILABLE");

        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.CreatorNotFound>(result.AsT1);
    }

    [Fact]
    public async Task RateCreator_ShouldReturnTrue_WhenRatingIsSuccessful()
    {
        int rating = 5;
        int creatorId = 99;
        _fixture.SetupCreatorFound(_fixture.UserId);
        _fixture.SetupRateCreatorSuccess(_fixture.UserId);

        var result = await _fixture.CreatorService.RateCreator(creatorId, _fixture.UserId, rating);

        Assert.True(result.IsT0);
        Assert.True(result.AsT0);
    }

    [Fact]
    public async Task RateCreator_ShouldReturnErrorRatingCreator_WhenAlreadyRated()
    {
        int userId = 2;
        int creatorId = 1;
        int rating = 5;

        _fixture.SetupUserAlreadyRated(userId, creatorId);

        var result = await _fixture.CreatorService.RateCreator(creatorId, userId, rating);

        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.ErrorRatingCreator>(result.AsT1);
    }

    [Fact]
    public async Task AddSocialProfile_ShouldReturnPlatformNotFound_WhenPlatformDoesNotExist()
    {
        int platformId = 1;
        int userId = 1;
        string user_name = "Name";
        string link = "http://link.com";
        string description = "Desc";
        int followers = 10;
        decimal priceMin = 10;
        decimal priceMax = 20;

        _fixture.SetupPlatformNotFound(platformId);

        var result = await _fixture.CreatorService.AddSocialProfile(userId, null, user_name, link, description, followers, priceMin, priceMax, platformId, new List<string>());

        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.PlatformNotFound>(result.AsT1);
    }

    [Fact]
    public async Task AddSocialProfile_ShouldReturnInvalidSectorName_WhenSectorsDoNotMatch()
    {
        int platformId = 1;
        int userId = 1;
        string user_name = "Name";
        string link = "http://link.com";
        string description = "Desc";
        int followers = 10;
        decimal priceMin = 10;
        decimal priceMax = 20;

        _fixture.SetupSectorsCheckFailure(expectedCount: 2, actualCount: 1);

        var result = await _fixture.CreatorService.AddSocialProfile(userId, null, user_name, link, description, followers, priceMin, priceMax, platformId, new List<string> { "S1", "S2" });

        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.InvalidSectorName>(result.AsT1);
    }

    [Fact]
    public async Task EditCreatorSocialProfile_ShouldReturnProfileDoesntBelongToYou_WhenUserIdMismatch()
    {
        int userId = 99;
        int socialProfileId = 1;
        string user_name = "Name";
        string link = "http://link.com";
        string description = "Desc";
        int followers = 10;
        decimal priceMin = 10;
        decimal priceMax = 20;

       _fixture.SetupProfileById(socialProfileId, ownerId: 1);

        var result = await _fixture.CreatorService.EditCreatorSocialProfile(userId, null, socialProfileId, user_name, link, description, followers, priceMin, priceMax, new List<string>());

        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.ProfileDoesntBellongToYou>(result.AsT1);
    }

    [Fact]
    public async Task Search_ShouldReturnPagedResponse_WhenParametersAreProvided()
    {
        var pagedResponse = new PagedResponse<CreatorSocialProfile> { Items = new List<CreatorSocialProfile>() };
        _fixture.SetupSearchReturns(pagedResponse);

        var result = await _fixture.CreatorService.Search(null, null, null, null, null, null, null, 1, 10);

        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task GetTrendingCreators_ShouldReturnEmptyList_WhenNoHistory()
    {
        _fixture.SetupTrendingCreators(null);

        var result = await _fixture.CreatorService.GetTrendingCreators(5);

        Assert.True(result.IsT0);
        Assert.Empty(result.AsT0);
    }

    [Fact]
    public async Task RemoveSocialProfile_ShouldReturnNotFound_WhenProfileDoesNotBelongToUser()
    {
        int ownerId = 1;
        int hackerId = 2;
        int profileId = 10;

        _fixture.SetupProfileByIdWithOwnership(profileId, ownerId);

        var result = await _fixture.CreatorService.RemoveSocialProfile(hackerId, null, profileId);

        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.SocialProfileNotFound>(result.AsT1);
    }

}