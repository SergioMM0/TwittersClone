using AuthService.Application.Clients;
using RabbitMQMessages.Auth;

namespace AuthService.Core.Services; 
public class AuthenticationService {
    private readonly MessageClient _messageClient;

    public AuthenticationService(MessageClient messageClient) {
        _messageClient = messageClient;
    }

    public void AuthenticateUser(string username, string password) {
        _messageClient.Send(new LoginMsg() {
            Token = "token"
        }, "AuthService/login-response"
        );
    }
}
