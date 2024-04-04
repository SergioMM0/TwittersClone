namespace RabbitMQMessages.Follow;
public class FetchFollowersMsg {
    public required int UserId { get; set; }
    public required List<int> FollowerIds { get; set; }
}
