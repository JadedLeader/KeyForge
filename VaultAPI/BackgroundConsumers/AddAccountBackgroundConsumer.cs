
using AccountAPI.DataModel;
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await GetAccounts();
            }
        }


        private async Task GetAccounts()
        {

            StreamAccountRequest streamAccountsRequest = new StreamAccountRequest();    

            var streamAccountHandshake = _accountClient.StreamAccount(streamAccountsRequest);

            var responseStream = streamAccountHandshake.ResponseStream.ReadAllAsync();

            await foreach(var item in responseStream)
            {

                if(_addAccountResponse.Add(item))
                {
                    AccountDataModel accountModelToAdd = MapStreamAccountToDataModel(item);

                    await _accountRepo.AddAsync(accountModelToAdd); 
                }

            }


            throw new NotImplementedException();
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
