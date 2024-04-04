using API.Application.Clients;
using Microsoft.AspNetCore.Mvc;
using RabbitMQMessages.Like;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class LikeController : ControllerBase{
    private readonly MessageClient _messageClient;

    public LikeController(MessageClient messageClient){
        _messageClient = messageClient;
    }
    
    /// <summary>
    /// Creates a like for a post in the system
    /// </summary>
    /// <returns>string - If the post got created or not, it's title, author and comments</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Add([FromBody] AddLikeDto addLikeDto){
        var responseTask = _messageClient.ListenAsync<LikeAddedMsg>("API/like-added");

        _messageClient.Send(new AddLikeMsg(){
            PostId = addLikeDto.PostId,
            UserId = addLikeDto.UserId
        }, "LikeService/addLike");

        var response = await responseTask;

        if(!response.Success) {
            return BadRequest(response.Reason);
        }
        return Ok($"Like added successfully for post with id: {addLikeDto.PostId}");
    }
}

public class AddLikeDto{
    public int PostId { get; set; }
    public int UserId { get; set; }
}
