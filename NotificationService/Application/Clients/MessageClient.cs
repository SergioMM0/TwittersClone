using EasyNetQ;

namespace NotificationService.Application.Clients; 
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

    public void ListenAsync<T>(Func<T, Task> handler, string topic) {
        _bus.PubSub.SubscribeAsync<T>(topic, handler);
    }

    public void SendAsync<T>(T message, string topic) {
        _bus.PubSub.PublishAsync(message, topic);
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
