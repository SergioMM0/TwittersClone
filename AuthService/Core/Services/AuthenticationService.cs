using AuthService.Application.Clients;
using RabbitMQMessages.Login;

namespace AuthService.Core.Services; 
public class AuthenticationService {
    private readonly MessageClient _messageClient;

    public AuthenticationService(MessageClient messageClient) {
        _messageClient = messageClient;
    }
    
    public void GenerateTokenForUser(string msgUsername) {
        _messageClient.Send(new LoginMsg() {
                Token = $"Authorized user {msgUsername}"
            }, "Authentication/login-response"
        );
    }
}
