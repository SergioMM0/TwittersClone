using EasyNetQ;

namespace API.Application.Clients;

public class MessageClient {
    private readonly IBus _bus;
    private readonly HashSet<string> _subscriptions = new HashSet<string>();

    public MessageClient(IBus bus) {
        _bus = bus;
    }

    public void Send<T>(T message, string topic) {
        _bus.PubSub.Publish(message, topic);
    }

    public void Send(string topic) {
        _bus.PubSub.Publish(topic);
    }

    public void Listen<T>(Action<T> handler, string topic) {
        if (!_subscriptions.Add(topic)) {
            Console.WriteLine($"Already subscribed to topic {topic}. Ignoring duplicate subscription.");
            return;
        }

        _bus.PubSub.Subscribe<T>(Guid.NewGuid().ToString(), handler, x => x.WithTopic(topic));
    }

    public Task<T> ListenAsync<T>(string topic) {
        var tcs = new TaskCompletionSource<T>();
        var subscriptionId = Guid.NewGuid().ToString();

        IDisposable subscription = null!;

        subscription = _bus.PubSub.SubscribeAsync<T>(subscriptionId, message => {
            tcs.TrySetResult(message);
            subscription.Dispose();
        }, cfg => cfg.WithTopic(topic));

        return tcs.Task;
    }

}
