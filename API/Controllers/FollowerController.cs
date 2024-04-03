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
            var userExistsResponseTask = _messageClient.ListenAsync<CheckUserIdExistsMsg>("API/user-exists-response");
            _messageClient.Send(new CheckUserIdExistsReqMsg { UserId = newFollower.UserId }, "UserService/user-exists-request");
            var userExistsResponse = await userExistsResponseTask;

            if (!userExistsResponse.Exists) {
                return BadRequest("Couldn't find the user to follow");
            }

            // Add the follower since the user exists
            var followerAddedResponseTask = _messageClient.ListenAsync<AddFollowerMsg>("API/follower-added-response");
            _messageClient.Send(new AddFollowerReqMsg { UserId = newFollower.UserId, FollowerId = newFollower.FollowerId }, "FollowingService/follower-added-request");
            var followerAddedResponse = await followerAddedResponseTask;
            
            if (!followerAddedResponse.FollowerAdded) {
                return BadRequest("Couldn't add the follower, user already follows this person.");
            }

            return Ok("Follower added successfully");
    }
}

public class NewFollowerDto {
    public int UserId { get; set; }
    public int FollowerId { get; set; }
}