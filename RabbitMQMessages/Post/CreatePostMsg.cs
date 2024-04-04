namespace RabbitMQMessages.Post {
    public class CreatePostMsg {
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required int AuthorId { get; set; }
    }
}
