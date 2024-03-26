using API.Application.Clients;
using EasyNetQ;
using RabbitMQMessages.Auth;

namespace API.Application.Handlers;

public class AuthMessageHandler : BackgroundService {

    private void HandleResponseAuthMessage(LoginMsg msg) {
        Console.WriteLine("Received response: " + msg.Token);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));
        
        messageClient.Listen<LoginMsg>(HandleResponseAuthMessage, "AuthService/login-response");
        
        while(!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
