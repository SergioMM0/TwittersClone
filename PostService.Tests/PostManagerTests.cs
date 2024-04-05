using Microsoft.AspNetCore.Identity;
using Moq;
using PostService.Application.Interfaces.Clients;
using PostService.Application.Interfaces.Repositories;
using PostService.Core.Entities;
using PostService.Core.Services;
using RabbitMQMessages.Notification;
using RabbitMQMessages.Post;
using RabbitMQMessages.User;

namespace PostService.Tests;

public class PostManagerTests {
    private readonly Mock<IPostRepository> _postRepositoryMock = new Mock<IPostRepository>();
    private readonly Mock<IMessageClient> _messageClientMock = new Mock<IMessageClient>();
    private readonly PostManager _postManager;

    public PostManagerTests() {
        _postManager = new PostManager(_postRepositoryMock.Object);
    }


    [Fact]
    public async Task CreatePost_UserDoesNotExist_SendsFailureMessage() {
        // Arrange
        int authorId = 1;
        _messageClientMock.Setup(client => client.ListenAsync<ResponseUserExistsMsg>(It.IsAny<string>()))
            .ReturnsAsync(new ResponseUserExistsMsg { Success = false });

        // Act
        await _postManager.CreatePost("Test Title", "Test Body", authorId);

        // Assert
        _messageClientMock.Verify(client => client.Send(It.IsAny<PostCreatedMsg>(), "API/post-created"), Times.Once);
        // Further verify the contents of the sent message as needed
    }

    [Fact]
    public async Task CreatePost_CreationFails_SendsFailureMessage() {
        // Arrange
        int authorId = 1;
        _messageClientMock.Setup(client => client.ListenAsync<ResponseUserExistsMsg>(It.IsAny<string>()))
            .ReturnsAsync(new ResponseUserExistsMsg { Success = true });
        _postRepositoryMock.Setup(repo => repo.Add(It.IsAny<Post>())).Returns(value: null);

        // Act
        await _postManager.CreatePost("Test Title", "Test Body", authorId);

        // Assert
        _messageClientMock.Verify(client => client.Send(It.Is<PostCreatedMsg>(msg => !msg.Success), "API/post-created"), Times.Once);
    }

    [Fact]
    public async Task CreatePost_SuccessfulCreation_SendsSuccessMessageAndNotification() {
        // Arrange
        int authorId = 1;
        var post = new Post { Id = 1, Title = "Test Title", Body = "Test Body", AuthorId = authorId };
        _messageClientMock.Setup(client => client.ListenAsync<ResponseUserExistsMsg>(It.IsAny<string>()))
            .ReturnsAsync(new ResponseUserExistsMsg { Success = true });
        _postRepositoryMock.Setup(repo => repo.Add(It.IsAny<Post>())).Returns(post);

        // Act
        await _postManager.CreatePost(post.Title, post.Body, post.AuthorId);

        // Assert
        _messageClientMock.Verify(client => client.Send(It.Is<PostCreatedMsg>(msg => msg.Success), "API/post-created"), Times.Once);
    }
}
