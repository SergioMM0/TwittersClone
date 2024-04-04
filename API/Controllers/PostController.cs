﻿using API.Application.Clients;
using Microsoft.AspNetCore.Mvc;
using RabbitMQMessages.Post;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase{
    private readonly MessageClient _messageClient;

    public PostController(MessageClient messageClient){
        _messageClient = messageClient;
    }

    /// <summary>
    /// Creates a post in the system
    /// </summary>
    /// <returns>string - If the post got created or not, it's title, author and comments</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Create([FromBody] NewPostDto newPostDto){
        var responseTask = _messageClient.ListenAsync<PostCreatedMsg>("API/post-created");

        _messageClient.Send(new CreatePostMsg(){
            Title = newPostDto.Title,
            AuthorId = newPostDto.AuthorId,
            Body = newPostDto.Body
        }, "PostService/createPost");

        var response = await responseTask;

        if(!response.Success) {
            return BadRequest(response.Reason);
        }
        return Ok("Post with title: " + response.Title + ", id: " + response.Id + " was created successfully");
    }
    
    /// <summary>
    /// Creates a post in the system
    /// </summary>
    /// <returns>string - If the post got created or not, it's title, author and comments</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> DeletePost([FromBody] DeletePostDto deletePostDto){
        var responseTask = _messageClient.ListenAsync<PostDeletedMsg>("API/post-deleted");

        _messageClient.Send(new DeletePostMsg(){
            Id = deletePostDto.Id
        }, "PostService/deletePost");

        var response = await responseTask;

        if(!response.Success) {
            return BadRequest(response.Reason);
        }
        return Ok("Post with title: " + response.Title + " deleted successfully");
    }
}

public class NewPostDto{
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required int AuthorId { get; set; }

}

public class DeletePostDto{
    public required int Id { get; set; }
}
