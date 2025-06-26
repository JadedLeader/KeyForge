
using Grpc.Core;
using gRPCIntercommunicationService;
using VaultAPI.Repos;

namespace VaultAPI.BackgroundConsumers
{
    public class AddAccountBackgroundConsumer : BackgroundService
    {

        private readonly Account.AccountClient _accountClient;

        private readonly AccountRepo _accountRepo;

        private HashSet<StreamAccountResponse> _addAccountResponse = new HashSet<StreamAccountResponse>();

        public AddAccountBackgroundConsumer(Account.AccountClient accountClient, AccountRepo accountRepo)
        {
            _accountClient = accountClient;
            _accountRepo = accountRepo;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }


        private async Task GetAccounts()
        {

            StreamAccountRequest streamAccountsRequest = new StreamAccountRequest();    

            var streamAccountHandshake = _accountClient.StreamAccount(streamAccountsRequest);

            var responseStream = streamAccountHandshake.ResponseStream.ReadAllAsync();

            await foreach(var item in responseStream)
            {
                
            }


            throw new NotImplementedException();
        }
    }
}
