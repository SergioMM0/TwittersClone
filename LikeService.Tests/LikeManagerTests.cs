using LikeService.Application.Interfaces.Clients;
using LikeService.Application.Interfaces.Repositories;
using LikeService.Core.Entities;
using LikeService.Core.Services;
using Moq;
using RabbitMQMessages.Like;
using RabbitMQMessages.Notification;
using RabbitMQMessages.Post;
using RabbitMQMessages.User;

namespace LikeService.Tests;

public class LikeManagerTests {
    private readonly Mock<ILikeRepository> _likeRepositoryMock = new Mock<ILikeRepository>();
    private readonly Mock<IMessageClient> _messageClientMock = new Mock<IMessageClient>();
    private readonly LikeManager _likeManager;

    public LikeManagerTests() {
        _likeManager = new LikeManager(_likeRepositoryMock.Object, _messageClientMock.Object);
    }
    
    [Fact]
    public async Task AddLike_UserDoesNotExist_SendsFailureMessage()
    {
        // Arrange
        _messageClientMock.Setup(client => client.ListenAsync<ResponseUserExistsMsg>(It.IsAny<string>()))
            .ReturnsAsync(new ResponseUserExistsMsg { Success = false });

        // Act
        await _likeManager.AddLike(1, 1);

        // Assert
        _messageClientMock.Verify(client => client.Send(It.Is<LikeAddedMsg>(msg => !msg.Success && msg.Reason == "User with given id not found"), "API/like-added"), Times.Once);
    }
    
    [Fact]
    public async Task AddLike_PostDoesNotExist_SendsFailureMessage()
    {
        // Arrange: Simulate user exists
        _messageClientMock.SetupSequence(client => client.ListenAsync<ResponseUserExistsMsg>(It.IsAny<string>()))
            .ReturnsAsync(new ResponseUserExistsMsg { Success = true });
        // Arrange: Simulate post does not exist
        _messageClientMock.Setup(client => client.ListenAsync<ResponsePostExists>(It.IsAny<string>()))
            .ReturnsAsync(new ResponsePostExists { Success = false });

        // Act
        await _likeManager.AddLike(1, 1);

        // Assert
        _messageClientMock.Verify(client => client.Send(It.Is<LikeAddedMsg>(msg => !msg.Success && msg.Reason == "Post with given id not found"), "API/like-added"), Times.Once);
    }
    
    [Fact]
    public async Task AddLike_LikeCreationFails_SendsFailureMessage()
    {
        // Arrange
        _messageClientMock.Setup(client => client.ListenAsync<ResponseUserExistsMsg>(It.IsAny<string>()))
            .ReturnsAsync(new ResponseUserExistsMsg { Success = true });
        _messageClientMock.Setup(client => client.ListenAsync<ResponsePostExists>(It.IsAny<string>()))
            .ReturnsAsync(new ResponsePostExists { Success = true });
        _likeRepositoryMock.Setup(repo => repo.Add(It.IsAny<Like>())).Returns(value: null); // Simulate failure

        // Act
        await _likeManager.AddLike(1, 1);

        // Assert
        _messageClientMock.Verify(client => client.Send(It.Is<LikeAddedMsg>(msg => !msg.Success && msg.Reason == "Couldn't create like in database"), "API/like-added"), Times.Once);
    }

    [Fact]
    public async Task AddLike_Success_SendsSuccessMessageAndNotification()
    {
        // Arrange
        var like = new Like { PostId = 1, UserId = 1 };
        _messageClientMock.Setup(client => client.ListenAsync<ResponseUserExistsMsg>(It.IsAny<string>()))
            .ReturnsAsync(new ResponseUserExistsMsg { Success = true });
        _messageClientMock.Setup(client => client.ListenAsync<ResponsePostExists>(It.IsAny<string>()))
            .ReturnsAsync(new ResponsePostExists { Success = true });
        _likeRepositoryMock.Setup(repo => repo.Add(It.IsAny<Like>())).Returns(like);

        // Act
        await _likeManager.AddLike(like.PostId, like.UserId);

        // Assert
        _messageClientMock.Verify(client => client.Send(It.Is<LikeAddedMsg>(msg => msg.Success), "API/like-added"), Times.Once);
        _messageClientMock.Verify(client => client.Send(It.IsAny<SendLikeNotifReqMsg>(), "NotificationService/send-like-notification-request"), Times.Once);
    }


}