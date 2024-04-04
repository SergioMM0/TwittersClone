﻿
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
    private readonly DatabaseContext _dbContext;
    
    public PostManager(PostRepository postRepository, MessageClient messageClient, DatabaseContext dbContext) {
        _postRepository = postRepository;
        _messageClient = messageClient;
        _dbContext = dbContext;
    }

    public async void CreatePost(string title, string body, int authorId) {
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
        
        var result = await _postRepository.CreateAsync(post);

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
}
