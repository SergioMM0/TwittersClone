namespace RabbitMQMessages.Notification;
public class SendLikeNotifMsg {
    public int UserId { get; set; }
    public int AuthorId { get; set; }
    public int PostId { get; set; }
    public required string Message { get; set; }
}