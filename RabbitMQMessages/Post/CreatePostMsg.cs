namespace RabbitMQMessages.Post;

public class CreatePostMsg
{
    public required string Body { get; set; }
    public required string Username { get; set; }
}