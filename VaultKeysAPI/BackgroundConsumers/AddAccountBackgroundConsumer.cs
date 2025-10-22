
using Grpc.Core;
using gRPCIntercommunicationService;
using Microsoft.Identity.Client;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class AddAccountBackgroundConsumer : BackgroundService
    {

        private Account.AccountClient _accountClient;

        private HashSet<Guid> _addAccountResponse = new HashSet<Guid>();

        private readonly IServiceScopeFactory _serviceScope;

        public AddAccountBackgroundConsumer(Account.AccountClient accountClient, IServiceScopeFactory scopeFactory)
        {
            _accountClient = accountClient;
            _serviceScope = scopeFactory;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        private async Task AddAccountsFromStreamAsync()
        {
            StreamAccountRequest newStreamAccountRequest = new StreamAccountRequest();

            var streamAccountClient = _accountClient.StreamAccount(newStreamAccountRequest);

            var accountsResponseStream = streamAccountClient.ResponseStream.ReadAllAsync();

            await foreach(var account in accountsResponseStream)
            {
                if (_addAccountResponse.Add(Guid.Parse(account.AccountId))){

                    

                }
            }

            throw new NotImplementedException();
            

        }
    }
}
