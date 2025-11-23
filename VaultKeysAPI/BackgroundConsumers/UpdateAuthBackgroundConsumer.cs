
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Serilog;
using VaultKeysAPI.Interfaces;
using VaultKeysAPI.Repos;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class UpdateAuthBackgroundConsumer : GenericGrpcConsumer<StreamAuthUpdatesResponse, AuthDataModel>
    {

        private readonly Auth.AuthClient _authClient;

        public UpdateAuthBackgroundConsumer(Auth.AuthClient authClient, IServiceScopeFactory serviceScope) : base(serviceScope)
        {
            _authClient = authClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, AuthDataModel model)
        {
            var scope = service.GetRequiredService<IAuthRepo>();

            Log.Information($"{nameof(UpdateAuthBackgroundConsumer)}: Received update with ID: {model.AuthKey} ");

            await scope.UpdateAsync(model);
        }

        protected override AuthDataModel MapToType(StreamAuthUpdatesResponse responseType)
        {
            return MapAuthUpdateToModel(responseType);
        }

        protected override IAsyncEnumerable<StreamAuthUpdatesResponse> OpenStream()
        {
            var client = _authClient.StreamAuthKeyUpdates(new StreamAuthUpdatesRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        private AuthDataModel MapAuthUpdateToModel(StreamAuthUpdatesResponse updates)
        {
            AuthDataModel model = new AuthDataModel
            {
                AccountId = Guid.Parse(updates.AccountId),
                ShortLivedKey = updates.ShortLivedKey,
                LongLivedKey = updates.LongLivedKey,
                AuthKey = Guid.Parse(updates.AuthKey),

            };

            return model;
        }
    }
}
