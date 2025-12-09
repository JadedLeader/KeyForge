using Grpc.Core;
using gRPCIntercommunicationService;
using Serilog;
using System.Collections.Immutable;
using TeamVaultAPI.Interfaces.Services;
using TeamVaultAPI.Storage;

namespace TeamVaultAPI.Services
{
    public class StreamingTeamVaultService : TeamVault.TeamVaultBase
    {

        private readonly StreamingStorage _streamingStorage;

        public StreamingTeamVaultService(StreamingStorage streamingStorage)
        {
            _streamingStorage = streamingStorage;
        }

        public override async Task StreamTeamVaultCreations(StreamTeamVaultCreationRequest request, IServerStreamWriter<StreamTeamVaultCreationResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {

                Log.Information("Client connected to vault creation stream");

                ImmutableList<StreamTeamVaultCreationResponse> vaultCreations = _streamingStorage.ReturnSteamTeamVaultCreations();

                foreach (StreamTeamVaultCreationResponse createdVault in vaultCreations)
                {

                    Log.Information($"Sending team vault to be created {createdVault.TeamVaultName}");

                    await responseStream.WriteAsync(createdVault);

                }

               // _streamingStorage.ClearStreamTeamVaultCreations();

                await Task.Delay(250, context.CancellationToken);

            }
        }

        public override async Task StreamTeamVaultDeletions(StreamTeamVaultDeletionsRequest request, IServerStreamWriter<StreamTeamVaultDeletionsResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                ImmutableList<StreamTeamVaultDeletionsResponse> vaultDeletions = _streamingStorage.ReturnStreamTeamVaultDeletions();

                foreach(StreamTeamVaultDeletionsResponse deletedVault in vaultDeletions)
                {

                    await responseStream.WriteAsync(deletedVault);

                }

                _streamingStorage.ClearStreamTeamVaultDeletions();

                await Task.Delay(250, context.CancellationToken);
            }
        }

        public override async Task StreamTeamVaultUpdates(StreamTeamVaultUpdatesRequest request, IServerStreamWriter<StreamTeamVaultUpdatesResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                ImmutableList<StreamTeamVaultUpdatesResponse> vaultUpdates = _streamingStorage.ReturnSteamTeamVaultUpdates();

                foreach(StreamTeamVaultUpdatesResponse updatedVault in vaultUpdates)
                {

                    await responseStream.WriteAsync(updatedVault);

                }

                _streamingStorage.ClearStreamTeamVaultUpdates();

                await Task.Delay(250, context.CancellationToken);
            }
        }

    }
}
