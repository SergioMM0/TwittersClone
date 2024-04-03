namespace RabbitMQMessages.Follow;
public class FetchFollowersMsg {
    public required List<int> UserIds { get; set; } = new List<int>();
}
