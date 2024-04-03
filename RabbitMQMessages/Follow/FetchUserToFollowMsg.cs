namespace RabbitMQMessages.Follow;
public class FetchUserToFollowMsg {
    public required int UserId { get; set; }
    public required string Username { get; set; }
}
