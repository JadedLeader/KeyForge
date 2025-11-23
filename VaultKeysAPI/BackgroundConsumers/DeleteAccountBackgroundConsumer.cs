
using KeyForgedShared.SharedDataModels;
using Grpc.Core;
using gRPCIntercommunicationService;
using VaultKeysAPI.Repos;
using VaultKeysAPI.Interfaces;
using KeyForgedShared.Generics;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class DeleteAccountBackgroundConsumer : GenericGrpcConsumer<StreamAccountDeleteResponse, AccountDataModel>
    {

        private readonly Account.AccountClient _accountClient;


        public DeleteAccountBackgroundConsumer(Account.AccountClient accountClient, IServiceScopeFactory serviceScope) : base(serviceScope) 
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
                AccountId = Guid.Parse(delete.AccountId),
            };

            return newAccount;
        }


    }
}
