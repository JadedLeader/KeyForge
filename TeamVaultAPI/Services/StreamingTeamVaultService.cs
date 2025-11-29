using Grpc.Core;
using gRPCIntercommunicationService;
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

                ImmutableList<StreamTeamVaultCreationResponse> vaultCreations = _streamingStorage.ReturnSteamTeamVaultCreations();

                foreach (StreamTeamVaultCreationResponse createdVault in vaultCreations)
                {

                    await responseStream.WriteAsync(createdVault);

                }

                _streamingStorage.ClearStreamTeamVaultCreations();

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
            }
        }

        public override Task StreamTeamVaultUpdates(StreamTeamVaultUpdatesRequest request, IServerStreamWriter<StreamTeamVaultUpdatesResponse> responseStream, ServerCallContext context)
        {
            return base.StreamTeamVaultUpdates(request, responseStream, context);
        }

    }
}
