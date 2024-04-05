namespace RabbitMQMessages.Follow;
public class AddFollowerMsg {
    public required int UserId { get; set; }
    public required int FollowerId { get; set; }
    public bool FollowerAdded { get; set; }
}
