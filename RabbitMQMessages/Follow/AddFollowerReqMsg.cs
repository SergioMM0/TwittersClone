namespace RabbitMQMessages.Follow;
public class AddFollowerReqMsg {
    public required int UserId { get; set; }
    public required int FollowerId { get; set; }
    public required bool ListenToNotifications { get; set; }
    public bool FollowerAdded { get; set; }
}
