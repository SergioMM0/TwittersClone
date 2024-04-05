using Moq;
using RabbitMQMessages.Login;
using RabbitMQMessages.User;
using UserService.Application.Interfaces.Clients;
using UserService.Application.Interfaces.Repositories;
using UserService.Core.Domain.Entities;
using UserService.Core.Services;

namespace UserService.Tests;

public class UserManagerTests {
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMessageClient> _messageClientMock;
    private readonly UserManager _userManager;
    
    public UserManagerTests() {
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
    
    [Fact]
    public void CreateUser_Success_SendsSuccessMessage()
    {
        // Arrange
        var username = "testUser";
        var password = "testPass";
        var user = new User { Username = username, Password = password };

        _userRepositoryMock.Setup(repo => repo.Create(It.IsAny<User>())).Returns(user);

        // Act
        _userManager.CreateUser(username, password);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<UserCreatedMsg>(msg => msg.Username == username && msg.Success),
            "API/user-created"), Times.Once);
    }
    
    [Fact]
    public void CreateUser_Failure_SendsFailureMessage()
    {
        // Arrange
        var username = "testUser";
        var password = "testPass";

        _userRepositoryMock.Setup(repo => repo.Create(It.IsAny<User>())).Returns(value: null); // Simulate failure

        // Act
        _userManager.CreateUser(username, password);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<UserCreatedMsg>(msg => msg.Username == username && !msg.Success),
            "API/user-created"), Times.Once);
    }
    
    [Fact]
    public void GetById_UserExists_SendsSuccessMessage()
    {
        // Arrange
        var user = new User { Id = 1, Username = "testUser", Password = "testPassword"};
        _userRepositoryMock.Setup(repo => repo.GetById(user.Id)).Returns(user);

        // Act
        _userManager.GetById(user.Id);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<UserMsg>(msg => msg.Success && msg.Id == user.Id && msg.Username == user.Username),
            "API/getUser-response"), Times.Once);
    }

    [Fact]
    public void GetById_UserDoesNotExist_SendsFailureMessage()
    {
        // Arrange
        var userId = 1;
        _userRepositoryMock.Setup(repo => repo.GetById(userId)).Returns(value: null);

        // Act
        _userManager.GetById(userId);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<UserMsg>(msg => !msg.Success),
            "API/getUser-response"), Times.Once);
    }
    
    [Fact]
    public void GetAllUsers_UsersExist_SendsUsersFoundMessage()
    {
        // Arrange
        var users = new List<User> { new User { Id = 1, Username = "User1", Password = "User1"}, new User { Id = 2, Username = "User2", Password = "User2"} };
        _userRepositoryMock.Setup(repo => repo.GetAllUsers()).Returns(users);

        // Act
        _userManager.GetAllUsers();

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<AllUsersMsg>(msg => msg.Any && msg.Users.Count == users.Count),
            "API/getAllUsers-response"), Times.Once);
    }

    [Fact]
    public void GetAllUsers_NoUsersExist_SendsNoUsersFoundMessage()
    {
        // Arrange
        var users = new List<User>();
        _userRepositoryMock.Setup(repo => repo.GetAllUsers()).Returns(users);

        // Act
        _userManager.GetAllUsers();

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<AllUsersMsg>(msg => !msg.Any),
            "API/getAllUsers-response"), Times.Once);
    }
    
    [Fact]
    public void CheckUserExists_UserExists_SendsExistenceConfirmation()
    {
        // Arrange
        var userId = 1;
        var receiverTopic = "receiver/topic";
        _userRepositoryMock.Setup(repo => repo.CheckUserExists(userId)).Returns(true);

        // Act
        _userManager.CheckUserExists(userId, receiverTopic);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<ResponseUserExistsMsg>(msg => msg.Success),
            receiverTopic), Times.Once);
    }

    [Fact]
    public void CheckUserExists_UserDoesNotExist_SendsNonExistenceConfirmation()
    {
        // Arrange
        var userId = 1;
        var receiverTopic = "receiver/topic";
        _userRepositoryMock.Setup(repo => repo.CheckUserExists(userId)).Returns(false);

        // Act
        _userManager.CheckUserExists(userId, receiverTopic);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<ResponseUserExistsMsg>(msg => !msg.Success),
            receiverTopic), Times.Once);
    }
}
