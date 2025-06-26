
using gRPCIntercommunicationService;

namespace VaultAPI.BackgroundConsumers
{
    public class AddAccountBackgroundConsumer : BackgroundService
    {

        private readonly Account.AccountClient _accountClient;

        public AddAccountBackgroundConsumer(Account.AccountClient accountClient)
        {
            _accountClient = accountClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
