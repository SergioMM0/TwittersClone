using NotificationService.Application.Clients;
using NotificationService.Core.Services;
using EasyNetQ;

namespace NotificationService.Application.Handlers; 
public class MessageHandler : BackgroundService {
    private readonly SendNotificationService _sendNotificationService;

    public MessageHandler(SendNotificationService sendNotificationService) {
        _sendNotificationService = sendNotificationService;
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
