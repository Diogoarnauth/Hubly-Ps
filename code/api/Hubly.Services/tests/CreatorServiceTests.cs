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

        var result = await _fixture.CreatorService.UpdateStatus(_fixture.UserId, invalidStatus);

        
        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.InvalidAvailabilityStatus>(result.AsT1);
    }

    [Fact]
    public async Task RateCreator_ShouldReturnTrue_WhenRatingIsSuccessful()
    {
        int rating = 5;
        _fixture.CreatorRepository.Setup(x => x.GetByUserId(_fixture.UserId)).ReturnsAsync(new Creator { Id = _fixture.UserId });
        _fixture.CreatorRepository.Setup(x => x.HasUserRatedCreator(It.IsAny<int>(), _fixture.UserId)).ReturnsAsync(false);
        _fixture.CreatorRepository.Setup(x => x.UpdateRating(It.IsAny<Creator>())).ReturnsAsync(true);

        var result = await _fixture.CreatorService.RateCreator(99, _fixture.UserId, rating);

        Assert.True(result.IsT0);
        Assert.True(result.AsT0);
    }

    [Fact]
    public async Task RemoveSocialProfile_ShouldReturnNotFound_WhenProfileDoesNotBelongToUser()
    {
        int ownerId = 1;
        int hackerId = 2;
        int profileId = 10;

        var profile = new CreatorSocialProfile { Id = profileId, CreatorId = ownerId };
        _fixture.CreatorSocialRepository.Setup(x => x.GetById(profileId)).ReturnsAsync(profile);

        var result = await _fixture.CreatorService.RemoveSocialProfile(hackerId, profileId);

        Assert.True(result.IsT1);
        Assert.IsType<CreatorError.SocialProfileNotFound>(result.AsT1);
    }

}