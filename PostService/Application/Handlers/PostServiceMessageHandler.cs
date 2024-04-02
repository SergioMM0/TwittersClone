using EasyNetQ;
using PostService.Application.Clients;
using PostService.Core.Services;
using RabbitMQMessages.Post;

namespace PostService.Application.Handlers;

public class PostServiceMessageHandler : BackgroundService
{
    private readonly PostManager _postManager;

    public PostServiceMessageHandler(PostManager postManager)
    {
        _postManager = postManager;
    }

    private void HandleCreatePost(CreatePostMsg msg)
    {
        _postManager.CreatePost(msg.Body, msg.Username);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));

        messageClient.Listen<CreatePostMsg>(HandleCreatePost, "PostService/createPost");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}