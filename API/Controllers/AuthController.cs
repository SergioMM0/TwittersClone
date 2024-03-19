using API.Application.Clients;
using Microsoft.AspNetCore.Mvc;
using RabbitMQMessages;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {

    private readonly MessageClient _messageClient;

    public AuthController(MessageClient messageClient) {
        _messageClient = messageClient;
    }

    [HttpPost]
    public IActionResult Login() {
        _messageClient.Send(
            new RequestAuthMsg {
                Username = "test",
                Password = "test"
            },
            "AuthenticateUser"
        );
        
        return Ok(true);
    }
}
