namespace RabbitMQMessages.Follow;
public class FetchFollowers {
    public required List<int> UserIds { get; set; } = new List<int>();
}
