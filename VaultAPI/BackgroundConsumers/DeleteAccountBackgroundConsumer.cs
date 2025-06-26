
using gRPCIntercommunicationService;

namespace VaultAPI.BackgroundConsumers
{
    public class DeleteAccountBackgroundConsumer : BackgroundService
    {

        private readonly Account.AccountClient _accountClient;

        public DeleteAccountBackgroundConsumer(Account.AccountClient accountClient)
        {
            _accountClient = accountClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
