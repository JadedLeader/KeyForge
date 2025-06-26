
using gRPCIntercommunicationService;
using gRPCIntercommunicationService.Protos;

namespace VaultAPI.BackgroundConsumers
{
    public class AddAuthBackgroundConsumer : BackgroundService
    {

        private readonly Auth.AuthClient _authClient;

        public AddAuthBackgroundConsumer(Auth.AuthClient authClient)
        {
            _authClient = authClient;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
}
