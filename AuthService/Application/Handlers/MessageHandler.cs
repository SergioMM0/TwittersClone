using AuthService.Application.Clients;
using EasyNetQ;
using RabbitMQMessages;


namespace AuthService.Application.Handlers {
    public class MessageHandler : BackgroundService {

        private void HandleRequestAuthMessage(RequestAuthMsg msg) {
            Console.WriteLine("Received request: " + msg.Username + " " + msg.Password);
        }
    
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            Console.WriteLine("Message handler is running...");
        
            var messageClient = new MessageClient(
                RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));
        
            messageClient.Listen<RequestAuthMsg>(HandleRequestAuthMessage, "authenticate");
        
            while(!stoppingToken.IsCancellationRequested) {
                await Task.Delay(1000, stoppingToken);
            }
            Console.WriteLine("Message handler is stopping...");
        }
    }
}
