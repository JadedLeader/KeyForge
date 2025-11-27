using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamVaultAPI.Interfaces.Repos;

namespace TeamVaultAPI.BackgroundConsumers
{
    public class DeleteAccount : GenericGrpcConsumer<StreamAccountDeleteResponse, AccountDataModel>
    {

        private readonly Account.AccountClient _accountClient;

        public DeleteAccount(Account.AccountClient accountClient, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            _accountClient = accountClient;
        }

        protected override Task HandleMessage(IServiceProvider service, AccountDataModel model)
        {
            var scope = service.GetRequiredService<IAccountRepo>();

            return scope.DeleteAsync(model);
        }

        protected override AccountDataModel MapToType(StreamAccountDeleteResponse responseType)
        {
            return MapResponseToModel(responseType);
        }

        protected override IAsyncEnumerable<StreamAccountDeleteResponse> OpenStream()
        {
            var client = _accountClient.StreamAccountDeletions(new StreamAccountDeleteRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        private AccountDataModel MapResponseToModel(StreamAccountDeleteResponse delete)
        {
            AccountDataModel newAccount = new AccountDataModel
            {
                Id = Guid.Parse(delete.AccountId),
            };

            return newAccount;
        }
    }
}
