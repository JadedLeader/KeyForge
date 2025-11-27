
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Serilog;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class DeleteVaultBackgroundConsumer : GenericGrpcConsumer<StreamVaultDeletionsResponse, VaultDataModel>
    {

        private readonly Vault.VaultClient _vaultClient;

        public DeleteVaultBackgroundConsumer(IServiceScopeFactory serviceScopeFactory, Vault.VaultClient vaultClient) : base(serviceScopeFactory)
        {
            _vaultClient = vaultClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, VaultDataModel model)
        {
            Log.Information($"{nameof(DeleteVaultBackgroundConsumer)}: received vault deletion {model.Id}");

            var scope = service.GetRequiredService<IVaultRepo>();

            await scope.DeleteVaultViaVaultId(model.Id);
        }

        protected override VaultDataModel MapToType(StreamVaultDeletionsResponse responseType)
        {
            return MapStreamToVault(responseType);
        }

        protected override IAsyncEnumerable<StreamVaultDeletionsResponse> OpenStream()
        {
            var client = _vaultClient.StreamVaultDeletions(new StreamVaultDeletionsRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        private VaultDataModel MapStreamToVault(StreamVaultDeletionsResponse vaultDeletion)
        {
            VaultDataModel newVault = new VaultDataModel
            {
                Id = Guid.Parse(vaultDeletion.VaultId)
            };


            return newVault;
        }
    }
}
