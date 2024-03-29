namespace RabbitMQMessages.Login;

public class LoginReqMsg
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}