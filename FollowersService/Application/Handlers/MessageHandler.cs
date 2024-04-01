using FollowersService.Application.Clients;
using FollowersService.Core.Services;
using EasyNetQ;
using RabbitMQMessages.Follow;

namespace FollowersService.Application.Handlers; 
public class MessageHandler : BackgroundService {
    private readonly FollowingService _followingService;

    public MessageHandler(FollowingService followingService) {
        _followingService = followingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));

        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
