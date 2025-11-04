
using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.SharedDataModels;
using Microsoft.Identity.Client;
using VaultKeysAPI.Interfaces;

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
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using IServiceScope createScope = _serviceScope.CreateScope();

            IAccountRepo getAccountRepoLifetime = createScope.ServiceProvider.GetRequiredService<IAccountRepo>();

            while (!stoppingToken.IsCancellationRequested)
            {
                await AddAccountsFromStreamAsync(getAccountRepoLifetime);
            }
        }

        private async Task AddAccountsFromStreamAsync(IAccountRepo accountRepo)
        {
            var callOptions = new CallOptions().WithWaitForReady();

            StreamAccountRequest newStreamAccountRequest = new StreamAccountRequest();

            var streamAccountClient = _accountClient.StreamAccount(newStreamAccountRequest, callOptions);

            var accountsResponseStream = streamAccountClient.ResponseStream.ReadAllAsync();

            await foreach(var account in accountsResponseStream)
            {
                if (!_addAccountResponse.Add(Guid.Parse(account.AccountId)))
                {
                    AccountDataModel newAccountDataModel = MapStreamAccountToDataModel(account);

                    await accountRepo.AddAsync(newAccountDataModel);
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
