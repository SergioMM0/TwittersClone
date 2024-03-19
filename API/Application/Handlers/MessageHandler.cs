using API.Application.Clients;
using EasyNetQ;
using RabbitMQMessages;

namespace API.Application.Handlers;

public class MessageHandler : BackgroundService {

    private void HandleResponseAuthMessage(ResponseAuthMsg msg) {
        Console.WriteLine("Received response: " + msg.IsAuthenticated);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));
        
        messageClient.Listen<ResponseAuthMsg>(HandleResponseAuthMessage, "auth-response");
        
        while(!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
