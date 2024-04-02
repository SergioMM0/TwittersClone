using RabbitMQMessages.Login;
using RabbitMQMessages.User;
using UserService.Application.Clients;
using UserService.Infrastructure.Repositories;
using UserService.Core.Domain.Entities;

namespace UserService.Core.Services;

public class UserManager {
    private readonly UserRepository _userRepository;
    private readonly MessageClient _messageClient;

    public UserManager(UserRepository userRepository, MessageClient messageClient) {
        _userRepository = userRepository;
        _messageClient = messageClient;
    }

    
    public void CheckUserExists(string username, string password) {
        Console.WriteLine("Checking username: " + username + " and password: " + password);
        var userExists = _userRepository.CheckUserExists(username);

        Console.WriteLine("The user exists: " + userExists);
        if (!userExists) {
            Console.WriteLine("User was not found... sending response to API");
            _messageClient.Send(new LoginMsg() {
                Token = "User not found"
            }, "Authentication/login-response");
            return;
        }
        Console.WriteLine("User was found... checking password...");

        var user = _userRepository.CheckPassword(username, password);

        if (user is null) {
            Console.WriteLine("Password incorrect... sending response to API");
            _messageClient.Send(new LoginMsg() {
                Token = "Incorrect password"
            }, "Authentication/login-response");
        }
        else {
            Console.WriteLine("User found and verified... sending request to AuthService");
            _messageClient.Send(new GenerateTokenMsg() {
                Username = username
            }, "AuthService/login-request");
        }
    }
    
    public void CreateUser(string username, string password) {
        Console.WriteLine("Creating user with username: " + username + " and password: " + password);
        var user = new User() {
            Username = username,
            Password = password
        };
        
        var result = _userRepository.Create(user);
        
        if (result is null) {
            Console.WriteLine("User creation failed... sending response to API");
            _messageClient.Send(new UserCreatedMsg() {
                Username = username,
                Success = false
            }, "API/UserCreated");
        }
        else {
            Console.WriteLine("User created successfully... sending response to API");
            _messageClient.Send(new UserCreatedMsg() {
                Username = username,
                Success = true
            }, "API/UserCreated");
        }
    }
    
    public void LocalTestAddUser() {
        var user = new User() {
            Username = "test",
            Password = "test"
        };

        _userRepository.Create(user);
    }
}
