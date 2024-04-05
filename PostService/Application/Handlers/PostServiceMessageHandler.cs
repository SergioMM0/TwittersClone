using EasyNetQ;
using PostService.Application.Clients;
using PostService.Core.Services;
using RabbitMQMessages.Post;

namespace PostService.Application.Handlers;

public class PostServiceMessageHandler : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;

    public PostServiceMessageHandler(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    private async void HandleCreatePost(CreatePostMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<PostManager>();

        Console.WriteLine($"{nameof(PostServiceMessageHandler)}: Creating post...");
         await userManager.CreatePost(msg.Title, msg.Body, msg.AuthorId);
    }
    
    private void HandleDeletePost(DeletePostMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<PostManager>();

        Console.WriteLine($"{nameof(PostServiceMessageHandler)}: Deleting post...");
        userManager.DeletePost(msg.Id);
    }
    
    private void HandleGetPostById(GetPostById msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<PostManager>();

        Console.WriteLine($"{nameof(PostServiceMessageHandler)}: Getting post by id: {msg.Id} ...");
        userManager.GetPostById(msg.Id);
    }
    
    private void HandleGetAllPost(GetAllPostMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<PostManager>();

        Console.WriteLine($"{nameof(PostServiceMessageHandler)}: Getting all posts...");
        userManager.GetAllPosts();
    }
    
    private void HandleCheckPostExists(PostExistsReqMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<PostManager>();

        Console.WriteLine($"{nameof(PostServiceMessageHandler)}: Checking if post exists...");
        userManager.CheckExists(msg.Id, msg.ReceiverTopic);
    }

    private void HandleGetPostAuthorById(GetPostAuthorById msg) {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<PostManager>();

        Console.WriteLine($"{nameof(PostServiceMessageHandler)}: Getting post author by id...");
        userManager.GetPostAuthorById(msg.Id);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));

        messageClient.Listen<CreatePostMsg>(HandleCreatePost, "PostService/createPost");
        
        messageClient.Listen<DeletePostMsg>(HandleDeletePost, "PostService/deletePost");
        
        messageClient.Listen<GetPostById>(HandleGetPostById, "PostService/getPostById");
        
        messageClient.Listen<GetAllPostMsg>(HandleGetAllPost, "PostService/getAllPosts");
        
        messageClient.Listen<PostExistsReqMsg>(HandleCheckPostExists, "PostService/checkPostExists");

        messageClient.Listen<GetPostAuthorById>(HandleGetPostAuthorById, "PostService/getPostAuthorById-request");
        
        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
