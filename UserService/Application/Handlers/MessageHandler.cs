using EasyNetQ;
using RabbitMQMessages.Auth;
using UserService.Application.Clients;
using UserService.Core.Services;

namespace UserService.Application.Handlers;

public class MessageHandler : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;

    public MessageHandler(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    private void HandleLoginRequest(LoginReqMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
        
        userManager.CheckUserExists(msg.Username, msg.Password);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));

        messageClient.Listen<LoginReqMsg>(HandleLoginRequest, "Authentication/login-request");

        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
