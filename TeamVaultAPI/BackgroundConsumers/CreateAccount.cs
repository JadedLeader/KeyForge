using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Serilog;
using TeamVaultAPI.Interfaces.Repos;

namespace TeamVaultAPI.BackgroundConsumers
{
    public class CreateAccount : GenericGrpcConsumer<StreamAccountResponse, AccountDataModel>
    {

        private readonly Account.AccountClient _accountClient;

        public CreateAccount(Account.AccountClient client, IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
            _accountClient = client;
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

            var serviceScope = service.GetRequiredService<IAccountRepo>();

            await serviceScope.AddAsync(model);
        }
    }
}
