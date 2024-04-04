using API.Application.Clients;
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
    /// Deletes a post in the system
    /// </summary>
    /// <returns>string - If the post got deleted or not and it's title</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Delete([FromBody] DeletePostDto deletePostDto){
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
    
    /// <summary>
    /// Gets a post by it's id
    /// </summary>
    /// <returns>string - The post</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> GetById([FromQuery] int id){
        var responseTask = _messageClient.ListenAsync<PostMsg>("API/GetPostById-response");

        _messageClient.Send(new GetPostById(){
            Id = id
        }, "PostService/getPostById");

        var response = await responseTask;

        if(!response.Success) {
            return BadRequest(response.Reason);
        }
        return Ok("Found post with title: " + response.Title + " and body: " + 
                  response.Body + " and authorId: " + response.AuthorId + " that belongs to id: " + response.Id);
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
