
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class DeleteVaultBackgroundConsumer : BackgroundService
    {

        private readonly HashSet<Guid> VaultsToDelete = new HashSet<Guid>();

        private readonly Vault.VaultClient _vaultClient;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DeleteVaultBackgroundConsumer(IServiceScopeFactory serviceScopeFactory, Vault.VaultClient vaultClient)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _vaultClient = vaultClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await ConsumeVaultDeletions();
            }
        }

        private async Task ConsumeVaultDeletions()
        {

            var callOptions = new CallOptions().WithWaitForReady();

            StreamVaultDeletionsRequest vaultDeletionsRequest = new StreamVaultDeletionsRequest();

            var stream = _vaultClient.StreamVaultDeletions(vaultDeletionsRequest);

            var responseStream = stream.ResponseStream.ReadAllAsync();

            await foreach(StreamVaultDeletionsResponse vaultDeletions in responseStream)
            {
                if (!VaultsToDelete.Add(Guid.Parse(vaultDeletions.VaultId)))
                {
                    IServiceScope newserviceScope = _serviceScopeFactory.CreateScope();

                    IVaultRepo vaultServiceScope = newserviceScope.ServiceProvider.GetRequiredService<IVaultRepo>();

                    Guid deleteVault = await vaultServiceScope.DeleteVaultViaVaultId(Guid.Parse(vaultDeletions.VaultId));

                    if(deleteVault ==  Guid.Empty)
                    {
                        throw new Exception($"delete vault in the background consumer failed as it doesn't exist");
                    }
                }

            }

        }
    }
}
