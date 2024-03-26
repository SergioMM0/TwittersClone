using AuthService.Application.Clients;
using AuthService.Core.Services;
using EasyNetQ;
using RabbitMQMessages.Auth;


namespace AuthService.Application.Handlers {
    public class MessageHandler : BackgroundService {
        private readonly AuthenticationService _authenticationService;
        
        public MessageHandler(AuthenticationService authenticationService) {
            _authenticationService = authenticationService;
        }

        private void HandleRequestAuthMessage(LoginReqMsg msg) {
            _authenticationService.AuthenticateUser(msg.Username, msg.Password);
        }
    
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            Console.WriteLine("Message handler is running...");
        
            var messageClient = new MessageClient(
                RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));
        
            messageClient.Listen<LoginReqMsg>(HandleRequestAuthMessage, "AuthService/login-request");
        
            while(!stoppingToken.IsCancellationRequested) {
                await Task.Delay(1000, stoppingToken);
            }
            Console.WriteLine("Message handler is stopping...");
        }
    }
}
