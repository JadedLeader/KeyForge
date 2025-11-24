
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.SharedDataModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Serilog;
using VaultKeysAPI.Interfaces;
using KeyForgedShared.Generics;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class AddVaultBackgroundConsumer : GenericGrpcConsumer<StreamVaultCreationsResponse, VaultDataModel>
    {

        private readonly Vault.VaultClient _vaultClient;

        public AddVaultBackgroundConsumer(IServiceScopeFactory scopeFactory, Vault.VaultClient vaultClient) : base(scopeFactory)
        {
            _vaultClient = vaultClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, VaultDataModel model)
        {
            var scope = service.GetRequiredService<IVaultRepo>();

            Log.Information($"{nameof(AddVaultBackgroundConsumer)}: received create vault request: {model.VaultId}");

            await scope.AddAsync(model);
        }

        protected override VaultDataModel MapToType(StreamVaultCreationsResponse responseType)
        {
            return MapVaultStreamToVaultModel(responseType);
        }

        protected override IAsyncEnumerable<StreamVaultCreationsResponse> OpenStream()
        {
            var client = _vaultClient.StreamVaultCreations(new StreamVaultCreationsRequest()); 

            return client.ResponseStream.ReadAllAsync();
        }

        private VaultDataModel MapVaultStreamToVaultModel(StreamVaultCreationsResponse vaultCreationsResponse)
        {
            VaultDataModel vaultStream = new VaultDataModel
            {
                AccountId = Guid.Parse(vaultCreationsResponse.AccountId),
                VaultCreatedAt = DateTime.Parse(vaultCreationsResponse.VaultCreatedAt),
                VaultId = Guid.Parse(vaultCreationsResponse.VaultId),
                VaultName = vaultCreationsResponse.VaultName,
                VaultType = (KeyForgedShared.SharedDataModels.VaultType)vaultCreationsResponse.VaultType,
            };

            return vaultStream;
        }
    }
}
