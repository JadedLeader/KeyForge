using Grpc.Core;
using gRPCIntercommunicationService;
using System.Collections.Immutable;
using TeamAPI.Storage;
using TeamAPI.Interfaces.Services;
using Serilog;

namespace TeamAPI.Services
{
    public class StreamingService : Team.TeamBase, IStreamingService
    {


        private readonly StreamingStorage _streamingStorage;
        public StreamingService(StreamingStorage streamingStorage)
        {
            _streamingStorage = streamingStorage;
        }

        public override async Task StreamTeamCreations(StreamTeamCreationRequest request, IServerStreamWriter<StreamTeamCreationResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {

                Log.Information($"Stream team creations online");

                ImmutableList<StreamTeamCreationResponse> teamCreations = _streamingStorage.ReturnTeamCreations();

                foreach(StreamTeamCreationResponse team in teamCreations)
                {
                    Log.Information($"{nameof(StreamingService)}: Sending create team request with team ID: {team.TeamId}");
                    await responseStream.WriteAsync(team);

                }

                await Task.Delay(250, context.CancellationToken);

            }
        }

        public override async Task StreamTeamDeletions(StreamTeamDeletionRequest request, IServerStreamWriter<StreamTeamDeletionResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                Log.Information($"Stream team deletions online");

                ImmutableList<StreamTeamDeletionResponse> teamDeletions = _streamingStorage.ReturnTeamDeletions();

                foreach(StreamTeamDeletionResponse deletion in teamDeletions)
                {

                    await responseStream.WriteAsync(deletion);

                }

                _streamingStorage.ClearTeamDeletions();

            }
        }

        public override async Task StreamTeamUpdates(StreamTeamUpdateRequest request, IServerStreamWriter<StreamTeamUpdateResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                Log.Information($"Stream team updates online");

                ImmutableList<StreamTeamUpdateResponse> teamUpdates = _streamingStorage.ReturnTeamUpdates();

                foreach(StreamTeamUpdateResponse update in teamUpdates)
                {
                    await responseStream.WriteAsync(update);

                }

                _streamingStorage.ClearTeamUpdates();
            }
        }

    }
}
