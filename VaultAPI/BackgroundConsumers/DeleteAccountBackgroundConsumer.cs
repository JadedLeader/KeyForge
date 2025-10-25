
using KeyForgedShared.SharedDataModels;
using Grpc.Core;
using gRPCIntercommunicationService;
using VaultAPI.Repos;
using VaultAPI.Interfaces.RepoInterfaces;

namespace VaultAPI.BackgroundConsumers
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
            
            using IServiceScope createScope = _serviceScope.CreateScope();

            IAccountRepo? getAccountRepoLifetime = createScope.ServiceProvider.GetRequiredService<IAccountRepo>();

            if (getAccountRepoLifetime == null)
            {
                throw new Exception($"{this.GetType().Namespace} Could not find the designated lifetime for the account repo");
            }

            while(!stoppingToken.IsCancellationRequested)
            {
                await DeleteAccounts(getAccountRepoLifetime);
            }
        }


        private async Task DeleteAccounts(IAccountRepo accountRepo)
        {

            StreamAccountDeleteRequest streamAccountDeletions = new StreamAccountDeleteRequest();   

            var handler = _accountClient.StreamAccountDeletions(streamAccountDeletions); 

            var responseStream = handler.ResponseStream.ReadAllAsync();

            await foreach(StreamAccountDeleteResponse deletion in responseStream)
            {
                if (_accountIds.Add(Guid.Parse(deletion.AccountId)))
                {
                    await accountRepo.DeleteAccountViaAccountId(Guid.Parse(deletion.AccountId));
                }
            }
        }

    }
}
