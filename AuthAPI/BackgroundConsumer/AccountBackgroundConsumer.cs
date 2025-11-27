
using KeyForgedShared.SharedDataModels;
using AuthAPI.Interfaces.RepoInterface;
using Serilog;
using Grpc.Core;
using gRPCIntercommunicationService;
using System.Threading;
using KeyForgedShared.Generics;

namespace AuthAPI.BackgroundConsumer
{
    public class AccountBackgroundConsumer :  GenericGrpcConsumer<StreamAccountResponse, AccountDataModel>
    {

        private readonly Account.AccountClient _accountClient;

        public AccountBackgroundConsumer(Account.AccountClient client, IServiceScopeFactory scopeFactory) : base(scopeFactory) 
        {
            _accountClient  = client;
           
        }

        protected override async Task HandleMessage(IServiceProvider service, AccountDataModel model)
        {
            Log.Information($"Received {model.Id} for creation within auth API");

            var accountRepo = service.GetRequiredService<IAuthRepo>();

            await accountRepo.AddAccountToTable(model);

        }

        protected override AccountDataModel MapToType(StreamAccountResponse responseType)
        {
            AccountDataModel newAccountDataModel = new AccountDataModel
            {
                Id = Guid.Parse(responseType.AccountId),
                Username = responseType.Username,
                Password = responseType.Password,
                Email = responseType.Email,
                AuthorisationLevel = (AuthRoles)responseType.AuthRole,
                AccountCreated = DateTime.Parse(responseType.AccountCreated),
            };

            return newAccountDataModel;
        }

        protected override IAsyncEnumerable<StreamAccountResponse> OpenStream()
        {
            var client = _accountClient.StreamAccount(new StreamAccountRequest());

            return client.ResponseStream.ReadAllAsync();
        }
    }
}
