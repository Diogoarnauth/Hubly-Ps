using Hubly.api.Services.Fixtures;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace Hubly.api.Services.Tests;

public class UserServiceTests : IClassFixture<UserServiceFixture>
{
    private readonly UserServiceFixture _fixture;

    public UserServiceTests(UserServiceFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks();
    }

    // Login / Token Tests
    [Fact]
    public async Task Token_WithValidCredentials_ShouldReturnToken()
    {
        var result = await _fixture.UserService.Token(_fixture.ValidEmail, _fixture.ValidPassword);

        Assert.True(result.IsT0);
        Assert.Equal(_fixture.TokenValue, result.AsT0);

        // Verifica o novo fluxo: primeiro busca por Email, depois verifica o Hash via BCrypt
        _fixture.UserRepository.Verify(x => x.GetUserByEmail(_fixture.ValidEmail), Times.Once);
        _fixture.PasswordEncoder.Verify(x => x.Verify(_fixture.ValidPassword, _fixture.HashedPassword), Times.Once);

        // Verifica a geração do Token
        _fixture.TokenService.Verify(x => x.GenerateTokenValue(), Times.Once);
        _fixture.TokenService.Verify(x => x.CreateTokenValidationInformation(_fixture.TokenValue), Times.Once);
        _fixture.TokenService.Verify(x => x.CreateToken(_fixture.UserId, _fixture.TokenValidation, It.IsAny<ITransactionContext>()), Times.Once);
    }

    [Fact]
    public async Task Token_WithInvalidCredentials_ShouldReturnFailure()
    {
        _fixture.SetupInvalidCredentials();

        var result = await _fixture.UserService.Token(_fixture.ValidEmail, _fixture.ValidPassword);

        Assert.True(result.IsT1);
        Assert.IsType<UserError.InvalidCredentials>(result.AsT1);

        // Deve tentar procurar e validar, mas falhar na password
        _fixture.UserRepository.Verify(x => x.GetUserByEmail(_fixture.ValidEmail), Times.Once);
        _fixture.PasswordEncoder.Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        // Nunca deve gerar token se falhar as credenciais
        _fixture.TokenService.Verify(x => x.GenerateTokenValue(), Times.Never);
    }

    /*[Fact]
    public async Task Token_WhenTokenUpdateFails_ShouldReturnFailure()
    {
        _fixture.SetupTokenUpdateFails();

        var result = await _fixture.UserService.Token(_fixture.ValidEmail, _fixture.ValidPassword);

        Assert.True(result.IsT1);
        Assert.IsType<UserError.FailedToUpdateToken>(result.AsT1);
    }*/

    // Register Tests
    [Fact]
    public async Task Register_WithValidInputs_ShouldReturnSuccess()
    {
        var result = await _fixture.UserService.Register(_fixture.ValidUsername, _fixture.ValidEmail, _fixture.ValidPassword);

        Assert.True(result.IsT0);
        Assert.NotNull(result.AsT0);
        Assert.Equal(_fixture.ValidEmail, result.AsT0.Email);
        Assert.Equal(_fixture.ValidUsername, result.AsT0.Name); // Ajustado para corresponder à propriedade 'Name' do teu User

        _fixture.UserRepository.Verify(r => r.UserExistsWithEmail(_fixture.ValidEmail), Times.Once);
        _fixture.UserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Once);
    }

    [Theory]
    [InlineData("short")] // Falha no tamanho mínimo de 8 caracteres configurado
    public async Task Register_WithInvalidPassword_ShouldReturnFailure(string password)
    {
        var result = await _fixture.UserService.Register(_fixture.ValidUsername, _fixture.ValidEmail, password);

        Assert.True(result.IsT1);
        Assert.IsType<UserError.InvalidPassword>(result.AsT1);

        _fixture.UserRepository.Verify(r => r.UserExistsWithEmail(It.IsAny<string>()), Times.Never);
        _fixture.UserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData("ab")] // Falha no tamanho mínimo de 3 caracteres configurado
    public async Task Register_WithInvalidUsername_ShouldReturnFailure(string username)
    {
        var result = await _fixture.UserService.Register(username, _fixture.ValidEmail, _fixture.ValidPassword);

        Assert.True(result.IsT1);
        Assert.IsType<UserError.InvalidName>(result.AsT1);

        _fixture.UserRepository.Verify(r => r.UserExistsWithEmail(It.IsAny<string>()), Times.Never);
        _fixture.UserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnFailure()
    {
        _fixture.SetupUserWithEmailExists();

        var result = await _fixture.UserService.Register(_fixture.ValidUsername, _fixture.ValidEmail, _fixture.ValidPassword);

        Assert.True(result.IsT1);
        Assert.IsType<UserError.EmailAlreadyExists>(result.AsT1);

        _fixture.UserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Register_WhenCreateUserFails_ShouldReturnFailure()
    {
        _fixture.SetupCreateUserFails();

        var result = await _fixture.UserService.Register(_fixture.ValidUsername, _fixture.ValidEmail, _fixture.ValidPassword);

        Assert.True(result.IsT1);
        Assert.IsType<UserError.FailedUserCreation>(result.AsT1);
    }

    [Fact]
    public async Task Register_WhenRepositoryThrowsException_ShouldHandleGracefully()
    {
        _fixture.SetupRepositoryThrowsException();

        await Assert.ThrowsAsync<System.Exception>(() =>
            _fixture.UserService.Register(_fixture.ValidUsername, _fixture.ValidEmail, _fixture.ValidPassword));

        _fixture.UserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
    }

    // Logout Tests
    [Fact]
    public async Task Logout_ShouldCallTokenServiceAndReturnSuccess()
    {
        var result = await _fixture.UserService.Logout(_fixture.TokenValue);

        Assert.True(result.IsT0);
        Assert.Equal("Logout successful", result.AsT0);

        _fixture.TokenService.Verify(t => t.DeleteToken(_fixture.TokenValue, It.IsAny<ITransactionContext>()), Times.Once);
    }

    [Fact]
    public async Task Logout_WhenTokenServiceThrowsException_ShouldHandleGracefully()
    {
        _fixture.SetupTokenServiceThrowsException();

        await Assert.ThrowsAsync<System.Exception>(() => _fixture.UserService.Logout(_fixture.TokenValue));

        _fixture.TokenService.Verify(t => t.DeleteToken(_fixture.TokenValue, It.IsAny<ITransactionContext>()), Times.Once);
    }

    // EditUser Tests
    [Fact]
    public async Task EditUser_ShouldCallRepositoryAndReturnSuccess()
    {
        var result = await _fixture.UserService.EditUser(_fixture.UserId, "newUsername");

        Assert.True(result.IsT0);
        Assert.True(result.AsT0);

        _fixture.UserRepository.Verify(r => r.EditUser(_fixture.UserId, "newUsername"), Times.Once);
    }

    // GetUserInfo Tests
    [Fact]
    public async Task GetUserInfo_WithValidUserId_ShouldReturnUser()
    {
        _fixture.SetupGetUserByIdSucceeds();

        var result = await _fixture.UserService.GetUserInfo(_fixture.UserId);

        Assert.True(result.IsT0);
        Assert.NotNull(result.AsT0);
        Assert.Equal(_fixture.UserId, result.AsT0.Id);

        _fixture.UserRepository.Verify(r => r.GetUserById(_fixture.UserId), Times.Once);
    }

    [Fact]
    public async Task GetUserInfo_WithInvalidUserId_ShouldReturnFailure()
    {
        _fixture.SetupGetUserByIdFails();

        var result = await _fixture.UserService.GetUserInfo(_fixture.UserId);

        Assert.True(result.IsT1);
        Assert.IsType<UserError.FailedToGetUserInfo>(result.AsT1);

        _fixture.UserRepository.Verify(r => r.GetUserById(_fixture.UserId), Times.Once);
    }

    // ChangePassword Tests
    [Fact]
    public async Task ChangePassword_WithValidInputs_ShouldReturnSuccess()
    {
        _fixture.SetupChangePasswordSucceeds();

        var result = await _fixture.UserService.ChangePassword(
            _fixture.UserId,
            "PasswordSegura123",
            "PasswordInsegura123"
        );

        Assert.True(result.IsT0);
        Assert.Equal("Password changed successfully", result.AsT0);

        _fixture.UserRepository.Verify(r => r.GetUserById(_fixture.UserId), Times.Once);
        _fixture.UserRepository.Verify(r => r.ChangePassword(_fixture.UserId, _fixture.HashedNewPassword), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_WithInvalidPassword_ShouldReturnFailure()
    {
        var result = await _fixture.UserService.ChangePassword(
            _fixture.UserId,
            "PasswordSegura123",
            "short"
        );

        Assert.True(result.IsT1);
        Assert.IsType<UserError.InvalidPassword>(result.AsT1);

        _fixture.UserRepository.Verify(r => r.GetUserById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_WithNonExistentUser_ShouldReturnFailure()
    {
        _fixture.SetupGetUserByIdFails();

        var result = await _fixture.UserService.ChangePassword(
            _fixture.UserId,
            "PasswordSegura123",
            "PasswordInsegura123"
        );

        Assert.True(result.IsT1);
        Assert.IsType<UserError.UserNotFound>(result.AsT1);

        _fixture.UserRepository.Verify(r => r.GetUserById(_fixture.UserId), Times.Once);
        _fixture.UserRepository.Verify(r => r.ChangePassword(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_WithIncorrectOldPassword_ShouldReturnFailure()
    {
        _fixture.SetupChangePasswordOldPasswordIncorrect();

        var result = await _fixture.UserService.ChangePassword(
            _fixture.UserId,
            "wrongOldPassword",
            "PasswordInsegura123"
        );

        Assert.True(result.IsT1);
        Assert.IsType<UserError.OldPasswordIsIncorrect>(result.AsT1);

        _fixture.UserRepository.Verify(r => r.GetUserById(_fixture.UserId), Times.Once);
        _fixture.UserRepository.Verify(r => r.ChangePassword(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_WithSameNewPassword_ShouldReturnFailure()
    {
        _fixture.SetupChangePasswordSamePassword();

        var result = await _fixture.UserService.ChangePassword(
            _fixture.UserId,
            "PasswordSegura123",
            "PasswordSegura123"
        );

        Assert.True(result.IsT1);
        Assert.IsType<UserError.NewPasswordCannotBeTheSameAsTheOldPassword>(result.AsT1);

        _fixture.UserRepository.Verify(r => r.GetUserById(_fixture.UserId), Times.Once);
        _fixture.UserRepository.Verify(r => r.ChangePassword(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }
}