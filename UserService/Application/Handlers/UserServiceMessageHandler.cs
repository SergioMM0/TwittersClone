using EasyNetQ;
using RabbitMQMessages.Login;
using RabbitMQMessages.User;
using UserService.Application.Clients;
using UserService.Core.Services;

namespace UserService.Application.Handlers;

public class UserServiceMessageHandler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public UserServiceMessageHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private void HandleLoginRequest(LoginReqMsg msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();

        Console.WriteLine("Checking user exists...");
        userManager.HandleLogin(msg.Username, msg.Password);
    }

    private void HandleCreateUser(CreateUserMsg msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
        
        Console.WriteLine($"{nameof(UserServiceMessageHandler)}: Creating user...");
        userManager.CreateUser(msg.Username, msg.Password);
    }
    
    private void HandleGetUserById(GetUserByIdMsg byIdMsg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
        
        Console.WriteLine($"{nameof(UserServiceMessageHandler)}: Finding user...");
        userManager.GetById(byIdMsg.Id);
    }
    
    private void HandleGetAllUsers(GetAllUsersMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
        
        Console.WriteLine($"{nameof(UserServiceMessageHandler)}: Retrieving all users...");
        userManager.GetAllUsers();
    }
    
    private void HandleCheckUserExists(UserExistsReqMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
        
        Console.WriteLine($"{nameof(UserServiceMessageHandler)}: Checking if user exists...");
        userManager.CheckUserExists(msg.Id, msg.ReceiverTopic);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));

        messageClient.Listen<LoginReqMsg>(HandleLoginRequest, "UserService/login-request");

        messageClient.Listen<CreateUserMsg>(HandleCreateUser, "UserService/create-user");
        
        messageClient.Listen<GetUserByIdMsg>(HandleGetUserById, "UserService/getUser");
        
        messageClient.Listen<GetAllUsersMsg>(HandleGetAllUsers, "UserService/getAllUsers");
        
        messageClient.Listen<UserExistsReqMsg>(HandleCheckUserExists, "UserService/checkUserExists");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
