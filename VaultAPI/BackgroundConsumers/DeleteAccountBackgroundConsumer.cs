
using AccountAPI.DataModel;
using Grpc.Core;
using gRPCIntercommunicationService;
using VaultAPI.Repos;

namespace VaultAPI.BackgroundConsumers
{
    public class DeleteAccountBackgroundConsumer : BackgroundService
    {

        private readonly Account.AccountClient _accountClient;

        private readonly AccountRepo _accountRepo;

        private HashSet<Guid> _accountIds = new HashSet<Guid>();

        public DeleteAccountBackgroundConsumer(Account.AccountClient accountClient, AccountRepo accountRepo)
        {
            _accountClient = accountClient;
            _accountRepo = accountRepo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await DeleteAccounts();
            }
        }


        private async Task DeleteAccounts()
        {

            StreamAccountDeleteRequest streamAccountDeletions = new StreamAccountDeleteRequest();   

            var handler = _accountClient.StreamAccountDeletions(streamAccountDeletions); 

            var responseStream = handler.ResponseStream.ReadAllAsync();

            await foreach(StreamAccountDeleteResponse deletion in responseStream)
            {
                if (_accountIds.Add(Guid.Parse(deletion.AccountId)))
                {
                    await _accountRepo.DeleteAccountViaAccountId(Guid.Parse(deletion.AccountId));
                }
            }
        }

    }
}
