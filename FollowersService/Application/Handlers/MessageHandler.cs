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

    private void HandleAddFollowerMessage(AddFollowerReqMsg msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var FollowingManager = scope.ServiceProvider.GetRequiredService<FollowingManager>();

        Console.WriteLine("Creating follower...");
        FollowingManager.AddFollower(msg.UserId, msg.FollowerId);
    }

    private void HandleFetchFollowersMessage(FetchFollowersReqMsg msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var FollowingManager = scope.ServiceProvider.GetRequiredService<FollowingManager>();

        Console.WriteLine("Fetching followers...");
        FollowingManager.FetchFollowers(msg.UserId);
    }

    private void HandleDeleteFollowerMessage(DeleteFollowerReqMsg msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var FollowingManager = scope.ServiceProvider.GetRequiredService<FollowingManager>();

        Console.WriteLine("Deleting follower...");
        FollowingManager.DeleteFollower(msg.UserId, msg.FollowerId);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));
        
        messageClient.Listen<AddFollowerReqMsg>(HandleAddFollowerMessage, "FollowingService/follower-added-request");
        messageClient.Listen<FetchFollowersReqMsg>(HandleFetchFollowersMessage, "FollowingService/fetch-followers-request");
        messageClient.Listen<DeleteFollowerReqMsg>(HandleDeleteFollowerMessage, "FollowingService/follower-deleted-request");

        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
