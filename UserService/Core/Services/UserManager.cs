using RabbitMQMessages.Auth;
using UserService.Application.Clients;
using UserService.Infrastructure.Repositories;

namespace UserService.Core.Services;

public class UserManager {
    private readonly UserRepository _userRepository;
    private readonly MessageClient _messageClient;
    
    public UserManager(UserRepository userRepository, MessageClient messageClient) {
        _userRepository = userRepository;
        _messageClient = messageClient;
    }

    public void CheckUserExists(string username, string password) {
        var userExists = _userRepository.CheckUserExists(username);
        
        if (!userExists) {
            _messageClient.Send(new LoginMsg() {
                Token = "User not found"
            }, "Authentication/login-response");
        }
        
        var user = _userRepository.CheckPassword(username, password);

        if (user is null) {
            _messageClient.Send(new LoginMsg() {
                Token = "Incorrect password"
            }, "Authentication/login-response");
        }
        else {
            _messageClient.Send(new LoginReqMsg() {
                Username = username,
                Password = password
            }, "AuthService/login-request");
        }
    }
}
