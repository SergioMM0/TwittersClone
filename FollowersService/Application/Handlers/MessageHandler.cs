using FollowersService.Application.Clients;
using FollowersService.Core.Services;
using EasyNetQ;
using RabbitMQMessages.Follow;

namespace FollowersService.Application.Handlers; 
public class MessageHandler : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;

    public MessageHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private void HandleAddFollowerMessage(AddFollowerMsg msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var FollowingManager = scope.ServiceProvider.GetRequiredService<FollowingManager>();

        Console.WriteLine("Creating follower...");
        FollowingManager.AddFollower(msg.UserId, msg.FollowerId);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));
        
        messageClient.Listen<AddFollowerMsg>(HandleAddFollowerMessage, "FollowingService/add-follower");

        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
