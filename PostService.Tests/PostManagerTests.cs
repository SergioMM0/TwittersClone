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
        _postManager = new PostManager(_postRepositoryMock.Object, _messageClientMock.Object);
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
        _messageClientMock.Verify(client => client.Send(It.IsAny<SendNotificationReqMsg>(), "NotificationService/send-notification-request"), Times.Once);
    }
    
    [Fact]
    public void DeletePost_PostDoesNotExist_SendsFailureMessage()
    {
        // Arrange
        int postId = 1;
        _postRepositoryMock.Setup(repo => repo.GetById(postId)).Returns(value: null);

        // Act
        _postManager.DeletePost(postId);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<PostDeletedMsg>(msg => msg.Success == false && msg.Reason == "Post with given id not found"),
            "API/post-deleted"), Times.Once);
    }
    
    [Fact]
    public void DeletePost_PostExists_SendsSuccessMessage()
    {
        // Arrange
        int postId = 2;
        var post = new Post { Id = postId, Title = "Existing Post", Body = "Content", AuthorId = 3 };
        _postRepositoryMock.Setup(repo => repo.GetById(postId)).Returns(post);

        // Act
        _postManager.DeletePost(postId);

        // Assert
        _postRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Post>()), Times.Once);
        _messageClientMock.Verify(client => client.Send(
            It.Is<PostDeletedMsg>(msg => msg.Success == true && msg.Title == post.Title),
            "API/post-deleted"), Times.Once);
    }

    [Fact]
    public void GetPostById_PostDoesNotExist_SendsFailureMessage()
    {
        // Arrange
        int postId = 1;
        _postRepositoryMock.Setup(repo => repo.GetById(postId)).Returns(value: null);

        // Act
        _postManager.GetPostById(postId);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<PostMsg>(msg => !msg.Success && msg.Reason == "Post with given id not found"),
            "API/GetPostById-response"), Times.Once);
    }

    [Fact]
    public void GetPostById_PostExists_SendsSuccessMessage()
    {
        // Arrange
        var post = new Post { Id = 1, Title = "Test Post", Body = "Test Body", AuthorId = 2 };
        _postRepositoryMock.Setup(repo => repo.GetById(post.Id)).Returns(post);

        // Act
        _postManager.GetPostById(post.Id);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<PostMsg>(msg => msg.Success && msg.Id == post.Id && msg.Title == post.Title && msg.Body == post.Body && msg.AuthorId == post.AuthorId),
            "API/GetPostById-response"), Times.Once);
    }

    [Fact]
    public void GetAllPosts_NoPostsFound_SendsFailureMessage()
    {
        // Arrange
        _postRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Post>());

        // Act
        _postManager.GetAllPosts();

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<AllPostMsg>(msg => !msg.Success && msg.Reason == "No posts found"),
            "API/getAllPosts-response"), Times.Once);
    }

    [Fact]
    public void GetAllPosts_PostsFound_SendsSuccessMessage()
    {
        // Arrange
        var posts = new List<Post>
        {
            new Post { Id = 1, Title = "Post 1", Body = "Body 1", AuthorId = 2 },
            new Post { Id = 2, Title = "Post 2", Body = "Body 2", AuthorId = 3 }
        };
        _postRepositoryMock.Setup(repo => repo.GetAll()).Returns(posts);

        // Act
        _postManager.GetAllPosts();

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<AllPostMsg>(msg => msg.Success && msg.Posts.Count == posts.Count),
            "API/getAllPosts-response"), Times.Once);
    }
    
    [Fact]
    public void GetPostAuthorById_PostDoesNotExist_SendsFailureMessage()
    {
        // Arrange
        int postId = 1;
        _postRepositoryMock.Setup(repo => repo.GetById(postId)).Returns(value: null);

        // Act
        _postManager.GetPostAuthorById(postId);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<PostAuthorMsg>(msg => !msg.Success && msg.AuthorId == -1 && msg.Reason == "Post with given id not found"),
            "Notification/getPostAuthorById-response"), Times.Once);
    }

    [Fact]
    public void GetPostAuthorById_PostExists_SendsSuccessMessage()
    {
        // Arrange
        int postId = 2;
        var post = new Post { Id = postId, AuthorId = 3, Title = "Title", Body = "Body" };
        _postRepositoryMock.Setup(repo => repo.GetById(postId)).Returns(post);

        // Act
        _postManager.GetPostAuthorById(postId);

        // Assert
        _messageClientMock.Verify(client => client.Send(
            It.Is<PostAuthorMsg>(msg => msg.Success && msg.AuthorId == post.AuthorId),
            "Notification/getPostAuthorById-response"), Times.Once);
    }


}
