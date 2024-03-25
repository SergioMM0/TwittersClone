using EasyNetQ;
using Microsoft.Extensions.Logging;
using System;

namespace UserService.Application.Clients
{
    public class MessageClient
    {
        private readonly IBus _bus;
        private readonly ILogger<MessageClient> _logger;

        public MessageClient(IBus bus, ILogger<MessageClient> logger)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public class MessageWrapper<T>
        {
            public T Message { get; set; }
            public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
            public MessageWrapper(T message)
            {
                Message = message;
                }
        }

        public void Send<T>(T message, string topic)
        {
            try
            {
                var wrappedMessage = new MessageWrapper<T>(message);
                _bus.PubSub.Publish(wrappedMessage, topic);
                _logger.LogInformation($"Message sent to topic '{topic}' with CorrelationId: {wrappedMessage.CorrelationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message to topic '{topic}'");
            }
        }

        public void Listen<T>(Action<T> handler, string subscriptionId)
        {
            try
            {
                _bus.PubSub.Subscribe<MessageWrapper<T>>(subscriptionId, wrapper => handler(wrapper.Message), x => x.WithTopic(subscriptionId));
                _logger.LogInformation($"Subscribed to topic '{subscriptionId}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error subscribing to topic '{subscriptionId}'");
            }
        }
    }
}
