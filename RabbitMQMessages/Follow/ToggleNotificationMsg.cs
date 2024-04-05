namespace RabbitMQMessages.Follow;
public class ToggleNotificationMsg {
    public int UserId { get; set; }
    public int FollowerId { get; set; }
    public bool ListenToNotifications { get; set; }
    public bool NotificationToggled { get; set; }
}
