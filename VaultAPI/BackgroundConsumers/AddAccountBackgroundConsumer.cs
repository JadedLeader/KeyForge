
using KeyForgedShared.SharedDataModels;
using Grpc.Core;
using gRPCIntercommunicationService;
using VaultAPI.Repos;
using VaultAPI.Interfaces.RepoInterfaces;
using KeyForgedShared.Generics;
using Microsoft.OpenApi.Writers;
using Serilog;

namespace VaultAPI.BackgroundConsumers
{
    public class AddAccountBackgroundConsumer : GenericGrpcConsumer<StreamAccountResponse, AccountDataModel>
    {

        private readonly Account.AccountClient _accountClient;

        public AddAccountBackgroundConsumer(Account.AccountClient accountClient, IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
            _accountClient = accountClient;
        }

       
        protected override AccountDataModel MapToType(StreamAccountResponse responseType)
        {
            AccountDataModel newAccountDataModel = new AccountDataModel
            {
                AccountId = Guid.Parse(responseType.AccountId),
                Username = responseType.Username,
                Password = responseType.Password,
                Email = responseType.Email,
                AccountCreated = DateTime.Parse(responseType.AccountCreated),
                AuthorisationLevel = (AuthRoles)responseType.AuthRole,

            };

            return newAccountDataModel;
        }

        protected override IAsyncEnumerable<StreamAccountResponse> OpenStream()
        {
            var client = _accountClient.StreamAccount(new StreamAccountRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        protected override async Task HandleMessage(IServiceProvider service, AccountDataModel model)
        {

            Log.Information($"Received {model.AccountId} to be created");

            var scope = service.GetRequiredService<IAccountRepo>();

            await scope.AddAsync(model);
        }
    }
}
