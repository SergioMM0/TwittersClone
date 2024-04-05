namespace RabbitMQMessages.Notification;

    public class SendNotificationReqMsg
    {
        public required int UserId { get; set; }
        public required int PostId { get; set; }
    }
