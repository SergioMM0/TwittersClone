using EasyNetQ;

namespace API.Application.Clients;

public class MessageClient {
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

    public Task<T> ListenAsync<T>(string topic) {
        var tcs = new TaskCompletionSource<T>();

        var subscriptionId = Guid.NewGuid().ToString();
        _bus.PubSub.SubscribeAsync<T>(subscriptionId, message => {
            tcs.TrySetResult(message);
        }, cfg => cfg.WithTopic(topic));

        return tcs.Task;
    }
}
