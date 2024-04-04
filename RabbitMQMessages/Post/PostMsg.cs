namespace RabbitMQMessages.Post {
    public class PostMsg {
        public required bool Success { get; set; }
        public string Reason { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int AuthorId { get; set; }
    }
}
