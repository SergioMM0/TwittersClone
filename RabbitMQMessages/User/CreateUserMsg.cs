namespace RabbitMQMessages.User {
    public class CreateUserMsg {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
