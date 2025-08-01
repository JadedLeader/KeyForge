
using gRPCIntercommunicationService;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class AddAccountBackgroundConsumer : BackgroundService
    {

        private Account.AccountClient _accountClient;

        public AddAccountBackgroundConsumer(Account.AccountClient accountClient)
        {
            _accountClient = accountClient;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        private async Task GetAccountsFromStream()
        {
            throw new NotImplementedException ();
        }
    }
}
