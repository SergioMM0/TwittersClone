using AuthService.Application.Clients;
using AuthService.Core.Services;
using EasyNetQ;
using RabbitMQMessages.Login;


namespace AuthService.Application.Handlers; 
public class MessageHandler : BackgroundService {
    private readonly AuthenticationService _authenticationService;

    public MessageHandler(AuthenticationService authenticationService) {
        _authenticationService = authenticationService;
    }

    private void HandleRequestAuthMessage(GenerateTokenMsg msg) {
        Console.WriteLine("Generating token for user {0}", msg.Username);
        _authenticationService.GenerateTokenForUser(msg.Username);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));

        messageClient.Listen<GenerateTokenMsg>(HandleRequestAuthMessage, "AuthService/login-request");

        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
