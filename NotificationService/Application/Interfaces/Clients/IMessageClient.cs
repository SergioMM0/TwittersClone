namespace NotificationService.Application.Interfaces.Clients;

public interface IMessageClient {
    void Send<T>(T message, string topic);
    void Listen<T>(Action<T> handler, string topic);
    void ListenAsync<T>(Func<T, Task> handler, string topic);
    void SendAsync<T>(T message, string topic);
    Task<T> ListenAsync<T>(string topic);
}
