
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.SharedDataModels;
using Serilog;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class UpdateVaultBackgroundConsumer : BackgroundService
    {

        private readonly Vault.VaultClient _vaultClient;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UpdateVaultBackgroundConsumer(IServiceScopeFactory serviceScopeFactory, Vault.VaultClient vaultClient)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _vaultClient = vaultClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ConsumeVaultUpdates();
        }

        private async Task ConsumeVaultUpdates()
        {
            Log.Information($"Update vault consumer connected");

            var stream = _vaultClient.StreamVaultUpdates(new StreamVaultUpdateRequest());

            await foreach(StreamVaultUpdateResponse vaultUpdateResponse in stream.ResponseStream.ReadAllAsync())
            {

                Log.Information($"Received vault update request from vault id {vaultUpdateResponse.VaultId} for key name: {vaultUpdateResponse.VaultName}");

                using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

                IVaultRepo vaultRepo = serviceScope.ServiceProvider.GetRequiredService<IVaultRepo>();

                VaultDataModel updatedModel = await vaultRepo.UpdateVaultKeyName(Guid.Parse(vaultUpdateResponse.VaultId), vaultUpdateResponse.VaultName);

                if(updatedModel.VaultName != vaultUpdateResponse.VaultName)
                {
                    Log.Information($"Key names aren't the same after update");
                }

            }

        }
    }
}
