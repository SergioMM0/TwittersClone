using API.Application.Clients;
using Microsoft.AspNetCore.Mvc;
using RabbitMQMessages.User;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase {

    private readonly MessageClient _messageClient;

    public UserController(MessageClient messageClient) {
        _messageClient = messageClient;
    }

    /// <summary>
    /// Creates an user in the system
    /// </summary>
    /// <returns>string - If the user got created or not and it's username</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Create([FromBody] NewUserDto newUserDto) {
        var responseTask = _messageClient.ListenAsync<UserCreatedMsg>("API/user-created");

        _messageClient.Send(new CreateUserMsg() {
            Username = newUserDto.Username,
            Password = newUserDto.Password
        }, "UserService/create-user");

        var response = await responseTask;

        if (response.Success) {
            return Ok("User with username: " + response.Username + " created successfully");
        }
        return BadRequest("Couldn't create the user");
    }
    
    /// <summary>
    /// Gets an user by it's id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetUserById([FromQuery] int id) {
        var responseTask = _messageClient.ListenAsync<UserMsg>("API/getUser-response");

        _messageClient.Send(new GetUserByIdMsg() {
            Id = id
        }, "UserService/getUser");

        var response = await responseTask;

        if (response.Success) {
            return Ok("User with id :" + response.Id + " was found. Username: " + response.Username);
        }
        return NotFound("Couldn't find the user");
    }
    
    /// <summary>
    /// Gets all users in the system
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAllUsers() {
        var responseTask = _messageClient.ListenAsync<AllUsersMsg>("API/getAllUsers-response");

        _messageClient.Send(new GetAllUsersMsg(), "UserService/getAllUsers");

        var response = await responseTask;

        if(!response.Any) {
            return NoContent();
        }

        var users = response.Users.Aggregate("", (current, user) => current + ("Id: " + user.Key + " Username: " + user.Value + "\n"));

        return Ok(users);
    }

    public class NewUserDto {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
