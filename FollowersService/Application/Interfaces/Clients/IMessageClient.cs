namespace FollowersService.Application.Interfaces.Clients;

public interface IMessageClient {
    void Send<T>(T message, string topic);
    void Listen<T>(Action<T> handler, string topic);
}
