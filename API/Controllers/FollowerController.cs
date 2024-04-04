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

    // get all followers of a user
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> GetFollowersAsync([FromQuery] int userId) {
        if (userId == 0) {
            return BadRequest("Invalid user ID.");
        }

        // Check if the user exists
        var userExistsResponseTask = _messageClient.ListenAsync<CheckUserIdExistsMsg>("API/user-exists-response");
        _messageClient.Send(new CheckUserIdExistsReqMsg { UserId = userId }, "UserService/user-exists-request");
        var userExistsResponse = await userExistsResponseTask;

        if (!userExistsResponse.Exists) {
            return BadRequest("Couldn't find the user to get followers");
        }

        // Get the followers of the user
        var fetchFollowersResponseTask = _messageClient.ListenAsync<FetchFollowersMsg>("API/fetch-followers-response");
        _messageClient.Send(new FetchFollowersReqMsg { UserId = userId }, "FollowingService/fetch-followers-request");
        var fetchFollowersResponse = await fetchFollowersResponseTask;

        if (fetchFollowersResponse.FollowerIds.Count == 0) {
            return Ok("Followers: 0");
        }

        return Ok(fetchFollowersResponse.FollowerIds);
    }
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> DeleteFollowerAsync([FromBody] NewFollowerDto newFollower) {
        if (newFollower.UserId == 0 || newFollower.FollowerId == 0) {
            return BadRequest("Invalid user or follower ID.");
        }

        // Check if the user exists
        var userExistsResponseTask = _messageClient.ListenAsync<CheckUserIdExistsMsg>("API/user-exists-response");
        _messageClient.Send(new CheckUserIdExistsReqMsg { UserId = newFollower.UserId }, "UserService/user-exists-request");
        var userExistsResponse = await userExistsResponseTask;

        if (!userExistsResponse.Exists) {
            return BadRequest("Couldn't find the user to delete follower");
        }

        // Delete the follower since the user exists
        var followerDeletedResponseTask = _messageClient.ListenAsync<DeleteFollowerMsg>("API/follower-deleted-response");
        _messageClient.Send(new DeleteFollowerReqMsg { UserId = newFollower.UserId, FollowerId = newFollower.FollowerId }, "FollowingService/follower-deleted-request");
        var followerDeletedResponse = await followerDeletedResponseTask;
        
        if (!followerDeletedResponse.FollowerDeleted) {
            return BadRequest("Couldn't delete the follower, user doesn't follow this person");
        }

        return Ok("Follower deleted successfully");
    }
}

public class NewFollowerDto {
    public int UserId { get; set; }
    public int FollowerId { get; set; }
}