
using AuthAPI.Interfaces.RepoInterface;
using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Microsoft.AspNetCore.Components.Forms;
using Serilog;
using System.Threading;

namespace AuthAPI.BackgroundConsumer
{
    public class DeleteAccountBackgroundConsumer : GenericGrpcConsumer<StreamAccountDeleteResponse, AccountDataModel>
    {

        private readonly Account.AccountClient _Accountclient;

        public DeleteAccountBackgroundConsumer(Account.AccountClient accountClient, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            _Accountclient = accountClient;
        }

        protected override AccountDataModel MapToType(StreamAccountDeleteResponse responseType)
        {
            AccountDataModel account = MapResponseToModel(responseType);

            return account;
        }

        protected override IAsyncEnumerable<StreamAccountDeleteResponse> OpenStream()
        {
            var client = _Accountclient.StreamAccountDeletions(new StreamAccountDeleteRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        protected override async Task HandleMessage(IServiceProvider service, AccountDataModel model)
        {
            Log.Information($"{typeof(DeleteAccountBackgroundConsumer)} recieved deleted request {model.Id}");

            var scope = service.GetRequiredService<IAuthRepo>();

            await scope.RemoveAccountFromTable(model);
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
