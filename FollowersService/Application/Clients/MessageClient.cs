using EasyNetQ;
using FollowersService.Application.Interfaces.Clients;

namespace FollowersService.Application.Clients; 
public class MessageClient : IMessageClient {
    private readonly IBus _bus;

    public MessageClient(IBus bus) {
        _bus = bus;
    }

    public void Send<T>(T message, string topic) {
        _bus.PubSub.Publish(message, topic);
    }

    public void Listen<T>(Action<T> handler, string topic) {
        _bus.PubSub.Subscribe<T>(topic, handler);
    }
}
