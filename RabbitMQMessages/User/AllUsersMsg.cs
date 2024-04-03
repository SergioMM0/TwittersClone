namespace RabbitMQMessages.User {
    public class AllUsersMsg {
        public required bool Any { get; set; }
        public Dictionary<int, string> Users { get; set; } = null!;
    }
}
