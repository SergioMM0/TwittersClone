namespace RabbitMQMessages.User {
    public class UserMsg {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public required bool Success { get; set; }
    }
}
