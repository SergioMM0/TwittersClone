using Moq;
using RabbitMQMessages.Login;
using UserService.Application.Interfaces.Clients;
using UserService.Application.Interfaces.Repositories;
using UserService.Core.Domain.Entities;
using UserService.Core.Services;

namespace UserService.Tests;

public class LoginTests {
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMessageClient> _messageClientMock;
    private readonly UserManager _userManager;
    
    public LoginTests() {
        _userRepositoryMock = new Mock<IUserRepository>();
        _messageClientMock = new Mock<IMessageClient>();
        _userManager = new UserManager(_userRepositoryMock.Object, _messageClientMock.Object);
    }
    
    [Fact]
    public void HandleLogin_UserDoesNotExist_SendsUserNotFoundMessage()
    {
        // Arrange
        var username = "nonExistingUser";
        var password = "anyPassword";
        _userRepositoryMock.Setup(repo => repo.CheckUserExists(username)).Returns(false);

        // Act
        _userManager.HandleLogin(username, password);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<LoginMsg>(msg => msg.Token == "User not found"),
            "Authentication/login-response"), Times.Once);
    }

    [Fact]
    public void HandleLogin_IncorrectPassword_SendsIncorrectPasswordMessage()
    {
        // Arrange
        var username = "existingUser";
        var password = "wrongPassword";
        _userRepositoryMock.Setup(repo => repo.CheckUserExists(username)).Returns(true);
        _userRepositoryMock.Setup(repo => repo.CheckPassword(username, password)).Returns(value: null); // Simulate incorrect password

        // Act
        _userManager.HandleLogin(username, password);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<LoginMsg>(msg => msg.Token == "Incorrect password"),
            "Authentication/login-response"), Times.Once);
    }
    
    [Fact]
    public void HandleLogin_CorrectCredentials_SendsLoginRequestToAuthService()
    {
        // Arrange
        var username = "validUser";
        var password = "correctPassword";
        _userRepositoryMock.Setup(repo => repo.CheckUserExists(username)).Returns(true);
        _userRepositoryMock.Setup(repo => repo.CheckPassword(username, password)).Returns(new User { Username = username, Password = password}); // Simulate successful login

        // Act
        _userManager.HandleLogin(username, password);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.IsAny<GenerateTokenMsg>(),
            "AuthService/login-request"), Times.Once);
    }
}
