namespace RabbitMQMessages.Notification;

public class ReceiveNotificationMsg
{
    public int UserId { get; set; } // User who will receive the notification
    public required string Message { get; set; }
    public bool NotificationSent { get; set; }
}
