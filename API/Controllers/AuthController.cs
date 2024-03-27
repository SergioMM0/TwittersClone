using API.Application.Clients;
using Microsoft.AspNetCore.Mvc;
using RabbitMQMessages.Auth;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {

    private readonly MessageClient _messageClient;

    public AuthController(MessageClient messageClient) {
        _messageClient = messageClient;
    }

    /// <summary>
    /// Logs the user in the system
    /// </summary>
    /// <returns>string - Authentication token</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]

    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto) {
        var responseTask = _messageClient.ListenAsync<LoginMsg>("Authentication/login-response");

        _messageClient.Send(new LoginReqMsg {
            Username = loginRequestDto.Username,
            Password = loginRequestDto.Password
        }, "UserService/login-request");

        var response = await responseTask;
        
        Console.WriteLine($"The API has received: {response.Token}");

        return response.Token switch {
            "Unauthorized" => BadRequest("Invalid credentials"),
            "User not found" => BadRequest("User not found"),
            "Incorrect password" => BadRequest("Incorrect password"),
            _ => Ok(response.Token)
        };

    }
}

public class LoginRequestDto {
    public string Username { get; set; }
    public string Password { get; set; }
}
