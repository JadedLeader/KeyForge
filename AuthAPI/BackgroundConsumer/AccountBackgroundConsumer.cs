
using AccountAPI.DataModel;
using AuthAPI.Interfaces.RepoInterface;
using Serilog;
using Grpc.Core;
using gRPCIntercommunicationService;

namespace AuthAPI.BackgroundConsumer
{
    public class AccountBackgroundConsumer :  IHostedService
    {

        private List<AccountDataModel> _accountsLocalStore = new();

        private readonly Account.AccountClient _accountClient;

        private readonly IServiceScopeFactory _scopeFactory;

        public AccountBackgroundConsumer(Account.AccountClient client, IServiceScopeFactory scopeFactory)
        {
            _accountClient  = client;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

                await ConsumeAccountChannelStream(cancellationToken);

                using IServiceScope scope = _scopeFactory.CreateScope();

                IAuthRepo authRepo = scope.ServiceProvider.GetRequiredService<IAuthRepo>();

                await AddAccountToAccountTable(authRepo);
           
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task ConsumeAccountChannelStream(CancellationToken cancellationToken)
        {
            StreamAccountRequest request = new StreamAccountRequest();

            using var call = _accountClient.StreamAccount(request, null, null, cancellationToken);

            var responseStream = call.ResponseStream;

            Log.Information($"Nothing in the queue, looping");

            while(await responseStream.MoveNext())
            {

                StreamAccountResponse accountDetails = responseStream.Current;

                Log.Information($"current item in response stream {accountDetails}");

                AccountDataModel mapToAccountModel = StreamResponseToAccountModel(accountDetails);

                if(_accountsLocalStore.Contains(mapToAccountModel))
                {
                   Log.Warning($"Local collection contains element account ID {mapToAccountModel.AccountId} skipping");
                }
                else if(!_accountsLocalStore.Contains(mapToAccountModel))
                {
                    _accountsLocalStore.Add(mapToAccountModel);
                }
            }
        }

        private AccountDataModel StreamResponseToAccountModel(StreamAccountResponse streamAccountResponse)
        {
            AccountDataModel newAccountDataModel = new AccountDataModel
            {
                AccountId = Guid.Parse(streamAccountResponse.AccountId),
                Username = streamAccountResponse.Username,
                Password = streamAccountResponse.Password,
                Email = streamAccountResponse.Email,
                AuthorisationLevel = (AuthRoles)streamAccountResponse.AuthRole,
                AccountCreated = DateTime.Parse(streamAccountResponse.AccountCreated),
            };

            return newAccountDataModel;
        }

        private async Task AddAccountToAccountTable(IAuthRepo authRepo)
        {
            if(_accountsLocalStore.Count == 0)
            {
                Log.Warning($"No existing entries within the local data structure for account models");
            }
            else
            {
                foreach (AccountDataModel localAccount in _accountsLocalStore)
                {
                    await authRepo.AddAccountToTable(localAccount);
                }

            }

        }

       
    }
}
