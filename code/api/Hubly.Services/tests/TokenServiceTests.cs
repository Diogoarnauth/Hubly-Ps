using Hubly.api.Services.Fixtures;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace Hubly.api.Services.Tests;

public class TokenServiceTests : IClassFixture<TokenServiceFixture>
{
    private readonly TokenServiceFixture _fixture;

    public TokenServiceTests(TokenServiceFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks();
    }

    [Fact]
    public void GenerateTokenValue_ShouldReturnBase64String()
    {
        var token = _fixture.TokenService.GenerateTokenValue();

        Assert.NotNull(token);
        Assert.True(token.Length > 0);
        Assert.True(IsBase64String(token));

        var expectedLength = 44;
        Assert.Equal(expectedLength, token.Length);
    }

    [Fact]
    public void GenerateTokenValue_ShouldGenerateUniqueTokens()
    {
        var token1 = _fixture.TokenService.GenerateTokenValue();
        var token2 = _fixture.TokenService.GenerateTokenValue();

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void CreateTokenValidationInformation_ShouldCallEncoder()
    {
        var result = _fixture.TokenService.CreateTokenValidationInformation(_fixture.TestToken);

        Assert.Equal(_fixture.HashedToken, result);

        _fixture.TokenEncoder.Verify(x => x.CreateValidationInformation(_fixture.TestToken), Times.Once);
    }

    [Fact]
    public async Task ValidateToken_WithValidToken_ShouldReturnUserId()
    {
        var userId = await _fixture.TokenService.ValidateToken(_fixture.TestToken, _fixture.TransactionContext.Object);

        Assert.NotNull(userId);
        Assert.Equal(_fixture.UserId, userId);

        _fixture.TokenEncoder.Verify(x => x.CreateValidationInformation(_fixture.TestToken), Times.Once);
        _fixture.TokenRepository.Verify(x => x.GetTokenByValidation(_fixture.HashedToken), Times.Once);
    }

    [Fact]
    public async Task ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        _fixture.SetupTokenValidationFails();

        var userId = await _fixture.TokenService.ValidateToken(_fixture.TestToken, _fixture.TransactionContext.Object);

        Assert.Null(userId);

        _fixture.TokenRepository.Verify(x => x.GetTokenByValidation(_fixture.HashedToken), Times.Once);
    }

    [Fact]
    public async Task CreateToken_ShouldCreateNewToken()
    {
        _fixture.SetupForNewTokenCreation();

        var result = await _fixture.TokenService.CreateToken(
            _fixture.UserId,
            _fixture.HashedToken,
            _fixture.TransactionContext.Object);

        Assert.True(result);
        _fixture.TokenRepository.Verify(x => x.CreateToken(It.IsAny<Token>()), Times.Once);
    }

    [Fact]
    public async Task DeleteToken_ShouldCallRepository()
    {
        var result = await _fixture.TokenService.DeleteToken(_fixture.TestToken, _fixture.TransactionContext.Object);

        Assert.True(result);

        _fixture.TokenRepository.Verify(x => x.DeleteTokenByValidation(_fixture.HashedToken), Times.Once);
    }

    [Fact]
    public async Task DeleteToken_WhenRepositoryFails_ShouldReturnFalse()
    {
        _fixture.SetupTokenDeletionFails();

        var result = await _fixture.TokenService.DeleteToken(_fixture.TestToken, _fixture.TransactionContext.Object);

        Assert.False(result);

        _fixture.TokenRepository.Verify(x => x.DeleteTokenByValidation(_fixture.HashedToken), Times.Once);
    }

    private bool IsBase64String(string base64)
    {
        try
        {
            var buffer = Convert.FromBase64String(base64);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
