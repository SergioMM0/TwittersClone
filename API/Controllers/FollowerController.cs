using API.Application.Clients;
using Microsoft.AspNetCore.Mvc;
using RabbitMQMessages.Follow;
using RabbitMQMessages.User;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class FollowerController : ControllerBase {

    private readonly MessageClient _messageClient;

    public FollowerController(MessageClient messageClient) {
        _messageClient = messageClient;
    }

    // add a new follower
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> AddFollowerAsync([FromBody] NewFollowerDto newFollower) {
            if (newFollower.UserId == 0 || newFollower.FollowerId == 0) {
                return BadRequest("Invalid user or follower ID.");
            }

            // Check if the user exists
            _messageClient.Send(new CheckUserIdExistsMsg { UserId = newFollower.UserId }, "UserService/check-existence");
            var userExistsResponse = await _messageClient.ListenAsync<CheckUserIdExistsMsg>("API/user-exists-response");

            if (!userExistsResponse.Exists) {
                return BadRequest("Couldn't find the user to follow");
            }

            // Add the follower since the user exists
            _messageClient.Send(new AddFollowerMsg { UserId = newFollower.UserId, FollowerId = newFollower.FollowerId }, "FollowingService/add-follower");
            var followerAddedResponse = await _messageClient.ListenAsync<AddFollowerMsg>("API/follower-added-response");
            
            if (!followerAddedResponse.FollowerAdded) {
                return BadRequest("Couldn't add the follower");
            }

            return Ok("Follower added successfully");
    }
}

public class NewFollowerDto {
    public int UserId { get; set; }
    public int FollowerId { get; set; }
}