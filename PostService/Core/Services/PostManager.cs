﻿
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using PostService.Application.Clients;
using PostService.Core.Entities;
using PostService.Infrastructure.Context;
using PostService.Infrastructure.Repositories;
using RabbitMQMessages.Post;
using RabbitMQMessages.User;

namespace PostService.Core.Services;

public class PostManager {
    private readonly PostRepository _postRepository;
    private readonly MessageClient _messageClient;
    private readonly DatabaseContext _db;
    
    public PostManager(PostRepository postRepository, DatabaseContext db) {
        _postRepository = postRepository;
        // Create a new message client for EF context issue
        _messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));
        _db = db;
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
}
