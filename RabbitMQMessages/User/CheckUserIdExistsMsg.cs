namespace RabbitMQMessages.User
{
    public class CheckUserIdExistsMsg
    {
        public required int UserId { get; set; }
        public bool Exists { get; set; }
    }
}
