
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Serilog;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class DeleteVaultBackgroundConsumer : BackgroundService
    {

        private readonly Vault.VaultClient _vaultClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DeleteVaultBackgroundConsumer(IServiceScopeFactory serviceScopeFactory, Vault.VaultClient vaultClient)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _vaultClient = vaultClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("DeleteVaultBackgroundConsumer started. Connecting to gRPC stream...");

            var request = new StreamVaultDeletionsRequest();
            var call = _vaultClient.StreamVaultDeletions(request, cancellationToken: stoppingToken);

            await foreach (var vaultDeletions in call.ResponseStream.ReadAllAsync(stoppingToken))
            {
                Log.Information($"Received vault delete request: {vaultDeletions.VaultId}");

                using var scope = _serviceScopeFactory.CreateScope();
                var vaultRepo = scope.ServiceProvider.GetRequiredService<IVaultRepo>();

                var deletedVault = await vaultRepo.DeleteVaultViaVaultId(Guid.Parse(vaultDeletions.VaultId));

                if (deletedVault != null)
                {
                    Log.Information($"Deleted vault: {deletedVault.VaultId} : {deletedVault.VaultName}");
                }
                else
                {
                   Log.Warning($"Vault {vaultDeletions.VaultId} not found in VaultKeysDB.");
                }
            }
        }
    }
}
