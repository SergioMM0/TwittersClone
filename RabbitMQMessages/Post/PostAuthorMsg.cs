namespace RabbitMQMessages.Post;
public class PostAuthorMsg {
    public required bool Success { get; set; }
    public string? Reason { get; set; }
    public required int AuthorId { get; set; }
}
