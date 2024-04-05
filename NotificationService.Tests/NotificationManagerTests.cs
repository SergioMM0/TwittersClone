using Moq;
using RabbitMQMessages.Notification;
using RabbitMQMessages.Post;
using RabbitMQMessages.Like;
using RabbitMQMessages.Follow;
using NotificationService.Core.Domain.Entities;
using NotificationService.Core.Services;
using NotificationService.Application.Interfaces.Clients;
using NotificationService.Application.Clients;

namespace NotificationService.Tests;

public class NotificationManagerTests
{
    private readonly Mock<IMessageClient> _messageClientMock;
    private readonly NotificationManager _notificationManager;
    
    public NotificationManagerTests() {
        _messageClientMock = new Mock<IMessageClient>();
        _notificationManager = new NotificationManager(_messageClientMock.Object);
    }

    [Fact]
    public void SendNotificationMsg_SuccessfullySendsNotification()
    {
        // Arrange
        var userId = 1;
        var postId = 1;
        var messageClientMock = new Mock<IMessageClient>();
        var notificationManager = new NotificationManager(messageClientMock.Object);

        // Act
        _ = notificationManager.SendNotificationMsg(userId, postId);

        // Assert
        messageClientMock.Verify(client => client.Send<FetchNotifListenersReqMsg>(
            It.IsAny<FetchNotifListenersReqMsg>(), 
            It.Is<string>(s => s == "FollowingService/fetch-notif-listeners-request")), 
            Times.Once);
    }

    [Fact]
    public void SendLikeNotifMsg_SuccessfullySendsLikeNotification()
    {
        // Arrange
        var userId = 1;
        var postId = 1;
        var messageClientMock = new Mock<IMessageClient>();
        var notificationManager = new NotificationManager(messageClientMock.Object);

        // Act
        _ = notificationManager.SendLikeNotifMsg(userId, postId);

        // Assert
        messageClientMock.Verify(client => client.Send<GetPostAuthorById>(
            It.IsAny<GetPostAuthorById>(), 
            It.Is<string>(s => s == "PostService/getPostAuthorById-request")), 
            Times.Once);
    }

    [Fact]
    public void SendNotificationMsg_WhenPostAuthorNotFound_ShouldNotSendNotification()
    {
        // Arrange
        var userId = 1;
        var postId = 1;
        var messageClientMock = new Mock<IMessageClient>();
        var notificationManager = new NotificationManager(messageClientMock.Object);

        // Act
        _ = notificationManager.SendLikeNotifMsg(userId, postId);

        // Assert
        messageClientMock.Verify(client => client.Send<GetPostAuthorById>(
            It.IsAny<GetPostAuthorById>(), 
            It.Is<string>(s => s == "PostService/getPostAuthorById-request")), 
            Times.Once);
    }

    [Fact]
    public void SendLikeNotifMsg_WhenPostAuthorFound_ShouldSendLikeNotification()
    {
        // Arrange
        var userId = 1;
        var postId = 1;
        var messageClientMock = new Mock<IMessageClient>();
        var notificationManager = new NotificationManager(messageClientMock.Object);

        // Act
        _ = notificationManager.SendLikeNotifMsg(userId, postId);

        // Assert
        messageClientMock.Verify(client => client.Send<GetPostAuthorById>(
            It.IsAny<GetPostAuthorById>(), 
            It.Is<string>(s => s == "PostService/getPostAuthorById-request")), 
            Times.Once);
    }
}