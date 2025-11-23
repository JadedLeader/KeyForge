using Grpc.Core;
using gRPCIntercommunicationService;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Serilog;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.BackgroundConsumers
{

    public class AddAuthBackgroundConsumer : GenericGrpcConsumer<StreamAuthCreationsResponse, AuthDataModel>
    {

        private readonly Auth.AuthClient _authClient;


        public AddAuthBackgroundConsumer(Auth.AuthClient authClient, IServiceScopeFactory serviceScope) : base(serviceScope)
        {
            _authClient = authClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, AuthDataModel model)
        {
            var scope = service.GetRequiredService<IAuthRepo>();

            await scope.AddAsync(model);
        }

        protected override AuthDataModel MapToType(StreamAuthCreationsResponse responseType)
        {
            return MapStreamAuthCreationToAuthModel(responseType);
        }

        protected override IAsyncEnumerable<StreamAuthCreationsResponse> OpenStream()
        {
            var client = _authClient.StreamAuthCreations(new StreamAuthCreationsRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        private AuthDataModel MapStreamAuthCreationToAuthModel(StreamAuthCreationsResponse streamAuthCreation)
        {
            AuthDataModel newAuthDataModel = new AuthDataModel
            {
                AuthKey = Guid.Parse(streamAuthCreation.AuthKey),
                AccountId = Guid.Parse(streamAuthCreation.AccountId),
                ShortLivedKey = streamAuthCreation.ShortLivedKey,
                LongLivedKey = streamAuthCreation.LongLivedKey,

            };

            return newAuthDataModel;


        }
    }

    
}

