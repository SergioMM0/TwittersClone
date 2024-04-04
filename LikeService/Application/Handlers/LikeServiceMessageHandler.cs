using EasyNetQ;
using LikeService.Application.Clients;
using LikeService.Core.Services;

namespace LikeService.Application.Handlers;

public class LikeServiceMessageHandler : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;

    public LikeServiceMessageHandler(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    /*
    private async void HandleCreateLike(CreateLikeMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<LikeManager>();

        Console.WriteLine($"{nameof(LikeServiceMessageHandler)}: Creating post...");
         //await userManager.CreateLike(msg.Title, msg.Body, msg.AuthorId);
    }

    */
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));

        //messageClient.Listen<CreateLikeMsg>(HandleCreateLike, "PostService/createPost");

        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}