namespace RabbitMQMessages.User;

public class UserExistsReqMsg {
    public required int Id { get; set; }
    public required string ReceiverTopic { get; set; }
}