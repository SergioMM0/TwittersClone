namespace RabbitMQMessages.Follow;
public class FetchNotifListenersMsg {
    public required int UserId { get; set; }
    public required List<int> NotifListeners { get; set; }
}
