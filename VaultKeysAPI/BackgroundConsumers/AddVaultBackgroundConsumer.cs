
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.SharedDataModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.BackgroundConsumers
{
    public class AddVaultBackgroundConsumer : BackgroundService
    {

        private readonly Vault.VaultClient _vaultClient;

        private readonly HashSet<Guid> vaultHashset = new HashSet<Guid>();

        private readonly IServiceScopeFactory _scopeFactory;
        public AddVaultBackgroundConsumer(IServiceScopeFactory scopeFactory, Vault.VaultClient vaultClient)
        {
            _scopeFactory = scopeFactory;
            _vaultClient = vaultClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        { 

            while (!stoppingToken.IsCancellationRequested)
            {
                await ConsumeVaultCreations();
            }
        }

        private async Task ConsumeVaultCreations()
        {
            var callOptions = new CallOptions().WithWaitForReady();

            StreamVaultCreationsRequest vaultCreationsRequest = new StreamVaultCreationsRequest();

            var vaultStream = _vaultClient.StreamVaultCreations(vaultCreationsRequest, callOptions);

            var vaultStreamResponses = vaultStream.ResponseStream.ReadAllAsync();

            await foreach(var vaultStreamResponse in vaultStreamResponses)
            {
                using IServiceScope createScope = _scopeFactory.CreateScope();

                IVaultRepo vaultRepo = createScope.ServiceProvider.GetRequiredService<IVaultRepo>();

                if (!vaultHashset.Add(Guid.Parse(vaultStreamResponse.VaultId)))
                {
                    VaultDataModel streamToVaultModel = MapVaultStreamToVaultModel(vaultStreamResponse);

                    await vaultRepo.AddAsync(streamToVaultModel);
                }
            }
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
