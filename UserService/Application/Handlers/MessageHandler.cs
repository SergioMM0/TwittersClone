using EasyNetQ;
using RabbitMQMessages.Login;
using RabbitMQMessages.User;
using UserService.Application.Clients;
using UserService.Core.Services;

namespace UserService.Application.Handlers;

public class MessageHandler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MessageHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private void CheckUserIdExists(CheckUserIdExistsMsg msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();

        Console.WriteLine("Checking existence of user...");
        userManager.CheckUserIdExists(msg.UserId);
    }

    private void HandleLoginRequest(LoginReqMsg msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();

        Console.WriteLine("Checking user exists...");
        userManager.CheckUserExists(msg.Username, msg.Password);
    }

    private void HandleCreateUser(CreateUserMsg msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
        
        Console.WriteLine($"{nameof(MessageHandler)}: Creating user...");
        userManager.CreateUser(msg.Username, msg.Password);
    }
    
    private void HandleGetUserById(GetUserByIdMsg byIdMsg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
        
        Console.WriteLine($"{nameof(MessageHandler)}: Finding user...");
        userManager.GetById(byIdMsg.Id);
    }
    
    private void HandleGetAllUsers(GetAllUsersMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
        
        Console.WriteLine($"{nameof(MessageHandler)}: Retrieving all users...");
        userManager.GetAllUsers();
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

        messageClient.Listen<CheckUserIdExistsMsg>(CheckUserIdExists, "UserService/check-existence");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
