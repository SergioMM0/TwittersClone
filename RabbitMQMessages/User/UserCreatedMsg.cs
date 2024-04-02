namespace RabbitMQMessages.User {
    public class UserCreatedMsg {
        public required bool Success { get; set; }
        public required string Username { get; set; }
    }
}
