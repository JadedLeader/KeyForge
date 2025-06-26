
using gRPCIntercommunicationService.Protos;

namespace VaultAPI.BackgroundConsumers
{
    public class UpdateAuthBackgroundConsumer : BackgroundService
    {

        private readonly Auth.AuthClient _authClient;

        public UpdateAuthBackgroundConsumer(Auth.AuthClient authClient)
        {
            _authClient = authClient;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
