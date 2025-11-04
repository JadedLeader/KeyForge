
using KeyForgedShared.SharedDataModels;
using Grpc.Core;
using gRPCIntercommunicationService;
using VaultAPI.Repos;
using VaultAPI.Interfaces.RepoInterfaces;

namespace VaultAPI.BackgroundConsumers
{
    public class AddAccountBackgroundConsumer : BackgroundService
    {

        private readonly Account.AccountClient _accountClient;

        private HashSet<Guid> _addAccountResponse = new HashSet<Guid>();

        private readonly IServiceScopeFactory _serviceScope;

        public AddAccountBackgroundConsumer(Account.AccountClient accountClient, IServiceScopeFactory serviceScope)
        {
            _accountClient = accountClient;
            _serviceScope  = serviceScope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using IServiceScope createScope = _serviceScope.CreateScope();

            IAccountRepo getAccountRepoLifetime = createScope.ServiceProvider.GetRequiredService<IAccountRepo>();

            while(!stoppingToken.IsCancellationRequested)
            {
                await GetAccounts(getAccountRepoLifetime);
            }
        }


        private async Task GetAccounts(IAccountRepo accountRepo)
        {
            var callOptions = new CallOptions().WithWaitForReady();

            StreamAccountRequest streamAccountsRequest = new StreamAccountRequest();    

            var streamAccountHandshake = _accountClient.StreamAccount(streamAccountsRequest, callOptions);

            var responseStream = streamAccountHandshake.ResponseStream.ReadAllAsync();

            await foreach(var item in responseStream)
            {

                if(_addAccountResponse.Add(Guid.Parse(item.AccountId)))
                {
                    AccountDataModel accountModelToAdd = MapStreamAccountToDataModel(item);

                    await accountRepo.AddAsync(accountModelToAdd); 
                }

            }

        }

        private AccountDataModel MapStreamAccountToDataModel(StreamAccountResponse accountResponse)
        {
            AccountDataModel newAccountDataModel = new AccountDataModel
            {
                AccountId = Guid.Parse(accountResponse.AccountId),
                Username = accountResponse.Username,
                Password = accountResponse.Password,
                Email = accountResponse.Email,
                AccountCreated = DateTime.Parse(accountResponse.AccountCreated),
                AuthorisationLevel = (AuthRoles)accountResponse.AuthRole,

            }; 

            return newAccountDataModel;
        }
    }
}
