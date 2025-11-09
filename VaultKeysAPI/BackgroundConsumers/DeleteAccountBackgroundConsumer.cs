
using KeyForgedShared.SharedDataModels;
using Grpc.Core;
using gRPCIntercommunicationService;
using VaultKeysAPI.Repos;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class DeleteAccountBackgroundConsumer : BackgroundService
    {

        private readonly Account.AccountClient _accountClient;

        private IServiceScopeFactory _serviceScope;

        private HashSet<Guid> _accountIds = new HashSet<Guid>();

        public DeleteAccountBackgroundConsumer(Account.AccountClient accountClient, IServiceScopeFactory serviceScope)
        {
            _accountClient = accountClient;
            _serviceScope = serviceScope;
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

            var callOptions = new CallOptions().WithWaitForReady();

            StreamAccountDeleteRequest streamAccountDeletions = new StreamAccountDeleteRequest();   

            var handler = _accountClient.StreamAccountDeletions(streamAccountDeletions, callOptions); 

            var responseStream = handler.ResponseStream.ReadAllAsync();

            await foreach(StreamAccountDeleteResponse deletion in responseStream)
            {
                using IServiceScope createScope = _serviceScope.CreateScope();

                IAccountRepo? accountRepo = createScope.ServiceProvider.GetRequiredService<IAccountRepo>();

                if (_accountIds.Add(Guid.Parse(deletion.AccountId)))
                {
                    await accountRepo.DeleteAccountViaAccountId(Guid.Parse(deletion.AccountId));
                }
            }
        }

    }
}
