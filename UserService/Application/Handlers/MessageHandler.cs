using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQMessages;
using System.Threading.Tasks;
using UserService.Application.Clients;

namespace UserService.Application.Handlers
{
    public class MessageHandler : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<MessageHandler> _logger;

        public MessageHandler(IServiceScopeFactory serviceScopeFactory, ILogger<MessageHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UserService Message handler is starting...");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var messageClient = scope.ServiceProvider.GetRequiredService<MessageClient>();
                messageClient.Listen<ResponseAuthMsg>(HandleResponseAuthMessage, "auth.response");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("UserService Message handler is stopping...");
        }

        private void HandleResponseAuthMessage(ResponseAuthMsg response)
        {
            // Here, include logic to handle the response, such as updating the user session
            _logger.LogInformation($"Received auth response: IsAuthenticated={response.IsAuthenticated}");
            // Further action based on the response
        }
    }
}
