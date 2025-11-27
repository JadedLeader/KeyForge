
using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Microsoft.Identity.Client;
using Serilog;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class AddAccountBackgroundConsumer : GenericGrpcConsumer<StreamAccountResponse, AccountDataModel>
    {

        private Account.AccountClient _accountClient;

        public AddAccountBackgroundConsumer(Account.AccountClient accountClient, IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
            _accountClient = accountClient;
        }

        protected override AccountDataModel MapToType(StreamAccountResponse responseType)
        {
            AccountDataModel newAccountDataModel = new AccountDataModel
            {
                Id = Guid.Parse(responseType.AccountId),
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
            Log.Information($"Received {model.Id} to be created");

            var serviceScope = service.GetRequiredService<IAccountRepo>();

            await serviceScope.AddAsync(model);
        }
    }
}
