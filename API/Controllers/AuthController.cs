﻿using API.Application.Clients;
using Microsoft.AspNetCore.Mvc;
using RabbitMQMessages;
using RabbitMQMessages.Auth;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {

    private readonly MessageClient _messageClient;

    public AuthController(MessageClient messageClient) {
        _messageClient = messageClient;
    }

    [HttpPost]
    public async Task<IActionResult> Login() {
        var responseTask = _messageClient.ListenAsync<LoginMsg>("AuthService/login-response");

        _messageClient.Send(new LoginReqMsg {
            Username = "test",
            Password = "test"
        }, "AuthService/login-request");

        var response = await responseTask;
        
        return Ok(response.Token);
    }
}
