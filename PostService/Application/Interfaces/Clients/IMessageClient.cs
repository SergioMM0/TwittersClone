namespace PostService.Application.Interfaces.Clients;

public interface IMessageClient {
    Task<T> ListenAsync<T>(string topic);
    void Listen<T>(Action<T> handler, string topic);
    void Send<T>(T message, string topic);
}