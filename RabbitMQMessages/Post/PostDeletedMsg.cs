namespace RabbitMQMessages.Post {
    public class PostDeletedMsg {
        public required bool Success { get; set; }
        public string Reason { get; set; }
        public string Title { get; set; }
    }
}
