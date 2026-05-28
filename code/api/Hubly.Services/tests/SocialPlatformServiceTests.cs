using Hubly.api.Services.Fixtures;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace Hubly.api.Services.Tests;

public class SocialPlatformServiceTests : IClassFixture<SocialPlatformServiceFixture>
{
    private readonly SocialPlatformServiceFixture _fixture;

    public SocialPlatformServiceTests(SocialPlatformServiceFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks();
    }


    [Fact]
    public async Task GetAllPlatforms_ShouldReturnList_WhenPlatformsExist()
    {
        
        var result = await _fixture.SocialPlatformService.GetAllPlatforms();

        Assert.True(result.IsT0);
        var platforms = result.AsT0;
        
        Assert.NotNull(platforms);
        Assert.Equal(6, platforms.Count); 
        
        _fixture.SocialPlatformRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetAllPlatforms_ShouldReturnError_WhenRepositoryReturnsNull()
    {
        _fixture.SetupGetAllPlatformsFails();

        var result = await _fixture.SocialPlatformService.GetAllPlatforms();

        Assert.True(result.IsT1); 
        Assert.IsType<SocialPlatformError.FailedToGetPlatforms>(result.AsT1);
        
        _fixture.SocialPlatformRepository.Verify(x => x.GetAll(), Times.Once);
    }
}
