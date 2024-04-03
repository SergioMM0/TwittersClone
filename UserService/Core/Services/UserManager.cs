using RabbitMQMessages.Login;
using RabbitMQMessages.User;
using UserService.Application.Clients;
using UserService.Infrastructure.Repositories;
using UserService.Core.Domain.Entities;

namespace UserService.Core.Services;

public class UserManager
{
    private readonly UserRepository _userRepository;
    private readonly MessageClient _messageClient;

    public UserManager(UserRepository userRepository, MessageClient messageClient)
    {
        _userRepository = userRepository;
        _messageClient = messageClient;
    }


    public void CheckUserExists(string username, string password)
    {
        Console.WriteLine("Checking username: " + username + " and password: " + password);
        var userExists = _userRepository.CheckUserExists(username);

        Console.WriteLine("The user exists: " + userExists);
        if (!userExists)
        {
            Console.WriteLine("User was not found... sending response to API");
            _messageClient.Send(new LoginMsg()
            {
                Token = "User not found"
            }, "Authentication/login-response");
            return;
        }
        Console.WriteLine("User was found... checking password...");

        var user = _userRepository.CheckPassword(username, password);

        if (user is null)
        {
            Console.WriteLine("Password incorrect... sending response to API");
            _messageClient.Send(new LoginMsg()
            {
                Token = "Incorrect password"
            }, "Authentication/login-response");
        }
        else
        {
            Console.WriteLine("User found and verified... sending request to AuthService");
            _messageClient.Send(new GenerateTokenMsg()
            {
                Username = username
            }, "AuthService/login-request");
        }
    }

    public void CreateUser(string username, string password)
    {
        Console.WriteLine("Creating user with username: " + username + " and password: " + password);
        var user = new User()
        {
            Username = username,
            Password = password
        };

        var result = _userRepository.Create(user);

        if (result is null)
        {
            Console.WriteLine("User creation failed... sending response to API");
            _messageClient.Send(new UserCreatedMsg()
            {
                Username = username,
                Success = false
            }, "API/user-created");
        }
        else
        {
            Console.WriteLine("User created successfully... sending response to API");
            _messageClient.Send(new UserCreatedMsg()
            {
                Username = username,
                Success = true
            }, "API/user-created");
        }
    }

    public void GetById(int id) {
        Console.WriteLine("Finding user with id: " + id);
        var user = _userRepository.GetById(id);

        if (user is null) {
            Console.WriteLine("User not found... sending response to API");
            _messageClient.Send(new UserMsg() {
                Success = false
            }, "API/getUser-response");
        }
        else {
            Console.WriteLine("User found... sending response to API");
            _messageClient.Send(new UserMsg() {
                Success = true,
                Id = id,
                Username = user.Username
            }, "API/getUser-response");
        }
    }

    public void LocalTestAddUser() {
        var user = new User() {
            Username = "test",
            Password = "test"
        };

        _userRepository.Create(user);
    }
    public void GetAllUsers() {
        Console.WriteLine("Retrieving all users...");
        var users = _userRepository.GetAllUsers();
        
        if (users.Count == 0) {
            Console.WriteLine("No users found... sending response to API");
            _messageClient.Send(new AllUsersMsg() {
                Any = false
            }, "API/getAllUsers-response");
        }
        else {
            Console.WriteLine("Users found... sending response to API");
            _messageClient.Send(new AllUsersMsg() {
                Any = true,
                Users = ListToMap(users)
            }, "API/getAllUsers-response");
        }
    }
    
    private Dictionary<int, string> ListToMap(List<User> users) {
        var map = new Dictionary<int, string>();
        foreach (var user in users) {
            map.Add(user.Id, user.Username);
        }
        return map;
    }
}
