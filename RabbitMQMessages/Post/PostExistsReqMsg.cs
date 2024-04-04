namespace RabbitMQMessages.Post;

public class PostExistsReqMsg {
    public required int Id { get; set; }
    public required string ReceiverTopic { get; set; }
}