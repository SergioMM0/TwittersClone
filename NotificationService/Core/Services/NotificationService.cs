using NotificationService.Application.Clients;

namespace NotificationService.Core.Services; 
public class SendNotificationService {
    private readonly MessageClient _messageClient;

    public SendNotificationService(MessageClient messageClient) {
        _messageClient = messageClient;
    }
}
