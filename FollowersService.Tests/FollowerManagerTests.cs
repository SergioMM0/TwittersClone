using Moq;
using RabbitMQMessages.Follow;
using FollowersService.Core.Domain.Entities;
using FollowersService.Core.Services;
using FollowersService.Application.Interfaces.Clients;
using FollowersService.Application.Interfaces.Repositories;

namespace FollowersService.Tests;

public class FollowerManagerTests {
    private readonly Mock<IFollowersRepository> _followersRepositoryMock;
    private readonly Mock<IMessageClient> _messageClientMock;
    private readonly FollowingManager _followingManager;
    
    public FollowerManagerTests() {
        _followersRepositoryMock = new Mock<IFollowersRepository>();
        _messageClientMock = new Mock<IMessageClient>();
        _followingManager = new FollowingManager(_messageClientMock.Object, _followersRepositoryMock.Object);
    }

    [Fact]
    public void AddFollowerAsync_SuccessfullyAddsFollowerAndSendsNotification()
    {
        // Arrange
        var follower = new Follower { FollowerId = 1, UserId = 1 };
        var messageClientMock = new Mock<IMessageClient>();
        var followersRepositoryMock = new Mock<IFollowersRepository>();
        followersRepositoryMock.Setup(repo => repo.Create(It.IsAny<Follower>())).Returns(follower);
        var followingManager = new FollowingManager(messageClientMock.Object, followersRepositoryMock.Object);

        // Act
        followingManager.AddFollower(follower.UserId, follower.FollowerId);

        // Assert
        Follower result = follower; // Add this line

        Assert.Equal(follower, result);
        messageClientMock.Verify(client => client.Send<AddFollowerMsg>(
            It.IsAny<AddFollowerMsg>(), 
            It.Is<string>(s => s == "API/follower-added-response")), 
            Times.Once);
    }

    [Fact]
    public void RemoveFollowerAsync_SuccessfullyRemovesFollowerAndSendsNotification()
    {
        // Arrange
        var followerId = 1;
        var userId = 1;
        var messageClientMock = new Mock<IMessageClient>();
        var followersRepositoryMock = new Mock<IFollowersRepository>();
        followersRepositoryMock.Setup(repo => repo.DeleteFollower(userId, followerId));
        var followingManager = new FollowingManager(messageClientMock.Object, followersRepositoryMock.Object);

        // Act
        followingManager.DeleteFollower(userId, followerId);

        // Assert
        followersRepositoryMock.Verify(repo => repo.DeleteFollower(userId, followerId), Times.Once);
        messageClientMock.Verify(client => client.Send<DeleteFollowerMsg>(
            It.IsAny<DeleteFollowerMsg>(),
            It.Is<string>(s => s == "API/follower-deleted-response")),
            Times.Once);
    }

    [Fact]
    public void FetchFollowersAsync_SuccessfullyFetchesFollowersAndSendsNotification()
    {
        // Arrange
        var userId = 1;
        var followers = new List<int> { 1, 2, 3 };
        var messageClientMock = new Mock<IMessageClient>();
        var followersRepositoryMock = new Mock<IFollowersRepository>();
        followersRepositoryMock.Setup(repo => repo.GetFollowers(userId)).Returns(followers);
        var followingManager = new FollowingManager(messageClientMock.Object, followersRepositoryMock.Object);

        // Act
        followingManager.FetchFollowers(userId);

        // Assert
        messageClientMock.Verify(client => client.Send<FetchFollowersMsg>(
            It.IsAny<FetchFollowersMsg>(),
            It.Is<string>(s => s == "API/fetch-followers-response")),
            Times.Once);
    }

    [Fact]
    public void AddFollowerAsync_FollowerAlreadyExists()
    {
        // Arrange
        var follower = new Follower { FollowerId = 1, UserId = 1 };
        var messageClientMock = new Mock<IMessageClient>();
        var followersRepositoryMock = new Mock<IFollowersRepository>();
        followersRepositoryMock.Setup(repo => repo.FollowerExists(follower.UserId, follower.FollowerId)).Returns(true);
        var followingManager = new FollowingManager(messageClientMock.Object, followersRepositoryMock.Object);

        // Act
        followingManager.AddFollower(follower.UserId, follower.FollowerId);

        // Assert
        messageClientMock.Verify(client => client.Send<AddFollowerMsg>(
            It.IsAny<AddFollowerMsg>(),
            It.Is<string>(s => s == "API/follower-added-response")),
            Times.Once);
    }

    [Fact]
    public void AddFollowerAsync_FollowerCreationFailed()
    {
        // Arrange
        var follower = new Follower { FollowerId = 1, UserId = 1 };
        var messageClientMock = new Mock<IMessageClient>();
        var followersRepositoryMock = new Mock<IFollowersRepository>();
        followersRepositoryMock.Setup(repo => repo.Create(It.IsAny<Follower>())).Returns(new Follower());
        var followingManager = new FollowingManager(messageClientMock.Object, followersRepositoryMock.Object);

        // Act
        followingManager.AddFollower(follower.UserId, follower.FollowerId);

        // Assert
        messageClientMock.Verify(client => client.Send<AddFollowerMsg>(
            It.IsAny<AddFollowerMsg>(),
            It.Is<string>(s => s == "API/follower-added-response")),
            Times.Once);
    }
    [Fact]
    public void RemoveFollowerAsync_FailedToRemoveFollower()
    {
        // Arrange
        var followerId = 1;
        var userId = 1;
        var messageClientMock = new Mock<IMessageClient>();
        var followersRepositoryMock = new Mock<IFollowersRepository>();
        followersRepositoryMock.Setup(repo => repo.DeleteFollower(userId, followerId)).Returns(false);
        var followingManager = new FollowingManager(messageClientMock.Object, followersRepositoryMock.Object);

        // Act
        followingManager.DeleteFollower(userId, followerId);

        // Assert
        messageClientMock.Verify(client => client.Send<DeleteFollowerMsg>(
            It.IsAny<DeleteFollowerMsg>(),
            It.Is<string>(s => s == "API/follower-deleted-response")),
            Times.Once);
    }

    [Fact]
    public void ToggleNotificationAsync_SuccessfullyTogglesNotification()
    {
        // Arrange
        var followerId = 1;
        var userId = 1;
        var newState = false;
        var messageClientMock = new Mock<IMessageClient>();
        var followersRepositoryMock = new Mock<IFollowersRepository>();
        followersRepositoryMock.Setup(repo => repo.ToggleNotification(userId, followerId, out newState)).Returns(true);
        var followingManager = new FollowingManager(messageClientMock.Object, followersRepositoryMock.Object);

        // Act
        followingManager.ToggleNotification(userId, followerId);

        // Assert
        messageClientMock.Verify(client => client.Send<ToggleNotificationMsg>(
            It.IsAny<ToggleNotificationMsg>(),
            It.Is<string>(s => s == "API/toggle-notification-response")),
            Times.Once);
    }
}
