namespace RabbitMQMessages.Post {
    public class AllPostMsg {
        public required bool Success { get; set; }
        public string Reason { get; set; }
        public Dictionary<int, string> Posts { get; set; }
    }
}
