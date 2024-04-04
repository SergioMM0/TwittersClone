namespace RabbitMQMessages.Like;

public class LikeAddedMsg {
    public required bool Success { get; set; }
    public string Reason { get; set; }
}