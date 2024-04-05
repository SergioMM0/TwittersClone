namespace RabbitMQMessages.Follow {
    public class DeleteFollowerMsg {
        public required int UserId { get; set; }
        public required int FollowerId { get; set; }
        public bool FollowerDeleted { get; set; }
    }
}