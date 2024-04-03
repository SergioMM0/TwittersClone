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
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetUserById([FromBody] GetUserDto newUserDto) {
        var responseTask = _messageClient.ListenAsync<UserMsg>("API/getUser-response");

        _messageClient.Send(new GetUserByIdMsg() {
            Id = newUserDto.Id
        }, "UserService/getUser");

        var response = await responseTask;

        if (response.Success) {
            return Ok("User with id :" + response.Id + " was found. Username: " + response.Username);
        }
        return NotFound("Couldn't find the user");
    }
    
    

    public class NewUserDto {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class GetUserDto {
        public required int Id { get; set; }
    }
}
