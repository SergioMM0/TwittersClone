namespace RabbitMQMessages.Follow {
    public class DeleteFollowerReqMsg {
        public required int UserId { get; set; }
        public required int FollowerId { get; set; }
        public bool FollowerDeleted { get; set; }
    }
}