using EasyNetQ;
using LikeService.Application.Interfaces.Clients;

namespace LikeService.Application.Clients {
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
}
