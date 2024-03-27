using EasyNetQ;
using RabbitMQMessages.Auth;
using UserService.Application.Clients;
using UserService.Core.Services;

namespace UserService.Application.Handlers;

public class MessageHandler : BackgroundService {
    private readonly UserManager _userManager;

    public MessageHandler(UserManager userManager) {
        _userManager = userManager;
    }

    private void HandleLoginRequest(LoginMsg msg) {
        _userManager.CheckCredentials(msg);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));

        messageClient.Listen<LoginMsg>(HandleLoginRequest, "UserService/login-request");

        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
