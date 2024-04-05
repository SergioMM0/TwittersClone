
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using PostService.Application.Clients;
using PostService.Application.Interfaces.Clients;
using PostService.Application.Interfaces.Repositories;
using PostService.Core.Entities;
using PostService.Infrastructure.Context;
using PostService.Infrastructure.Repositories;
using RabbitMQMessages.Post;
using RabbitMQMessages.User;
using RabbitMQMessages.Notification;

namespace PostService.Core.Services;

public class PostManager {
    private readonly IPostRepository _postRepository;
    private readonly IMessageClient _messageClient;
    
    public PostManager(IPostRepository postRepository) {
        _postRepository = postRepository;
        // Create a new message client for EF context issue
        _messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));
    }

    public async Task CreatePost(string title, string body, int authorId) {
        var receiverTopic = "PostService/checkUserExists-response";
        var task = _messageClient.ListenAsync<ResponseUserExistsMsg>(receiverTopic);
        
        Console.WriteLine("Checking that user with id: " + authorId + " exists...");
        
        _messageClient.Send(new UserExistsReqMsg()
        {
            Id = authorId,
            ReceiverTopic = receiverTopic
        }, "UserService/checkUserExists");
        
        var userExists = await task;
        
        Console.WriteLine("User exists: " + userExists.Success);

        if (!userExists.Success) {
            Console.WriteLine("User was not found... sending response to API");
            _messageClient.Send(new PostCreatedMsg()
            {
                Success = false,
                Reason = "User with given id not found"
            }, "API/post-created");
            return;
        }
        
        Console.WriteLine("Creating post with title: " + title + " , body: " + body + " and authorId: " + authorId);

        var post = new Post()
        {
            Title = title,
            Body = body,
            AuthorId = authorId
        };
        
        var result = _postRepository.Add(post);

        if (result is null)
        {
            Console.WriteLine("Post creation failed... sending response to API");
            _messageClient.Send(new PostCreatedMsg()
            {
                Success = false,
                Reason = "Couldn't create post in database"
            }, "API/post-created");
        }
        else
        {
            Console.WriteLine("User created successfully... sending response to API");
            _messageClient.Send(new PostCreatedMsg()
            {
                Success = true,
                Id = result.Id,
                Title = result.Title,
                Body = result.Body,
                AuthorId = result.AuthorId
            }, "API/post-created");

            // Send a notification to listeners of this user
            Console.WriteLine("Sending notification to listeners of user with id: " + authorId);
            _messageClient.Send(new SendNotificationReqMsg() {
                UserId = authorId,
                PostId = result.Id
            }, "NotificationService/send-notification-request");
        }
    }
    public void DeletePost(int postId) {
        var post = _postRepository.GetById(postId);
        
        if (post is null) {
            Console.WriteLine("Post with id: " + postId + " not found... sending response to API");
            _messageClient.Send(new PostDeletedMsg()
            {
                Success = false,
                Reason = "Post with given id not found"
            }, "API/post-deleted");
            return;
        }
        
        Console.WriteLine("Deleting post with title: " + post.Title + " , body: " + post.Body + " and authorId: " + post.AuthorId);

        _postRepository.Delete(post);
        
        Console.WriteLine("Post deleted successfully... sending response to API");
        _messageClient.Send(new PostDeletedMsg()
        {
            Success = true,
            Title = post.Title
        }, "API/post-deleted");
    }
    public void GetPostById(int postId) {
        var post = _postRepository.GetById(postId);
        
        if (post is null) {
            Console.WriteLine("Post with id: " + postId + " not found... sending response to API");
            _messageClient.Send(new PostMsg()
            {
                Success = false,
                Reason = "Post with given id not found"
            }, "API/GetPostById-response");
            return;
        }
        
        Console.WriteLine("Found post with title: " + post.Title + " and body: " + post.Body + " and authorId: " + post.AuthorId + " that belongs to id: " + post.Id);
        
        _messageClient.Send(new PostMsg()
        {
            Success = true,
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            AuthorId = post.AuthorId
        }, "API/GetPostById-response");
    }
    public void GetAllPosts() {
        var posts = _postRepository.GetAll();
        
        if (posts.Count == 0) {
            Console.WriteLine("No posts found... sending response to API");
            _messageClient.Send(new AllPostMsg()
            {
                Success = false,
                Reason = "No posts found"
            }, "API/getAllPosts-response");
            return;
        }
        
        Console.WriteLine("Found " + posts.Count + " posts");
        
        _messageClient.Send(new AllPostMsg()
        {
            Success = true,
            Posts = posts.ToDictionary(post => post.Id, post => post.Title)
        }, "API/getAllPosts-response");
    }
    public void CheckExists(int msgId, string msgReceiverTopic) {
        var post = _postRepository.GetById(msgId);
        
        if (post is null) {
            Console.WriteLine("Post with id: " + msgId + " not found... sending response back...");
            _messageClient.Send(new ResponsePostExists()
            {
                Success = false
            }, msgReceiverTopic);
            return;
        }
        
        Console.WriteLine("Post with id: " + msgId + " found... sending response back...");
        _messageClient.Send(new ResponsePostExists()
        {
            Success = true
        }, msgReceiverTopic);
    }
    public void GetPostAuthorById(int Id) {
        var post = _postRepository.GetById(Id);
        
        if (post is null) {
            Console.WriteLine("Post with id: " + Id + " not found...");
            _messageClient.Send(new PostAuthorMsg()
            {
                Success = false,
                Reason = "Post with given id not found",
                AuthorId = -1
            }, "Notification/getPostAuthorById-response");
            return;
        }
        
        Console.WriteLine("Found post with title: " + post.Title + " and body: " + post.Body + " and authorId: " + post.AuthorId + " that belongs to id: " + post.Id);
        
        _messageClient.Send(new PostAuthorMsg()
        {
            Success = true,
            Reason = "",
            AuthorId = post.AuthorId
        }, "Notification/getPostAuthorById-response");
    }
}
