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
    public async Task<IActionResult> Login() {
        var responseTask = _messageClient.ListenAsync<LoginMsg>("Authentication/login-response");

        _messageClient.Send(new LoginReqMsg {
            Username = "test",
            Password = "test"
        }, "UserService/login-request");

        var response = await responseTask;

        return response.Token switch {
            "Unauthorized" => BadRequest("Invalid credentials"),
            "User not found" => BadRequest("User not found"),
            "Incorrect password" => BadRequest("Incorrect password"),
            _ => Ok(response.Token)
        };

    }
}
