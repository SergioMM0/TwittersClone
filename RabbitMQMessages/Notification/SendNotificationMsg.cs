namespace RabbitMQMessages.Notification;

    public class SendNotificationMsg
    {
        public required int UserId { get; set; }
        public required List<int> FollowerId { get; set; }
        public required int PostId { get; set; }
        public required string Message { get; set; }
    }
