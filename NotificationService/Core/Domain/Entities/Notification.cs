namespace NotificationService.Core.Domain.Entities
{
    public class Notification{
        public required int Id { get; set; }
        public required int UserId { get; set; }
        public required int PostId { get; set; }
        public required string Message { get; set; }
    }
}
