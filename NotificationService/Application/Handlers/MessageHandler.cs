using NotificationService.Application.Clients;
using NotificationService.Core.Services;
using EasyNetQ;
using RabbitMQMessages.Notification;

namespace NotificationService.Application.Handlers; 
public class MessageHandler : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;

    public MessageHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private async Task HandleSendNotificationMessage(SendNotificationReqMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var notificationManager = scope.ServiceProvider.GetRequiredService<NotificationManager>();
        Console.WriteLine("Sending notification...");
        await notificationManager.SendNotificationMsg(msg.UserId, msg.PostId);
    }

    private async Task HandleSendLikeNotificationMessage(SendLikeNotifReqMsg msg) {
        using var scope = _scopeFactory.CreateScope();
        var notificationManager = scope.ServiceProvider.GetRequiredService<NotificationManager>();
        Console.WriteLine("Sending like notification...");
        await notificationManager.SendLikeNotifMsg(msg.UserId, msg.PostId);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("Message handler is running...");

        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));

        messageClient.ListenAsync<SendNotificationReqMsg>(HandleSendNotificationMessage,"NotificationService/send-notification-request");
        messageClient.ListenAsync<SendLikeNotifReqMsg>(HandleSendLikeNotificationMessage,"NotificationService/send-like-notification-request");

        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Message handler is stopping...");
    }
}
