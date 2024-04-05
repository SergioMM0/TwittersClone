namespace RabbitMQMessages.Like;

public class AddLikeMsg {
    public required int PostId { get; set; }
    public required int UserId { get; set; }
}
