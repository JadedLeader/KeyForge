using Grpc.Core;
using gRPCIntercommunicationService;
using Serilog;
using System.Collections.Immutable;
using TeamInviteAPI.StreamingStorage;

namespace TeamInviteAPI.Services
{
    public class TeamInviteStreamingService : TeamInvite.TeamInviteBase
    {

        private readonly TeamInviteStreamingStorage _streamingStorage;

        public TeamInviteStreamingService(TeamInviteStreamingStorage teamInviteStreamingStorage)
        {
            _streamingStorage = teamInviteStreamingStorage;
        }

        public override async Task StreamTeamInviteCreations(StreamTeamInviteCreationsRequest request, IServerStreamWriter<StreamTeamInviteCreationsResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {

                ImmutableList<StreamTeamInviteCreationsResponse> teamInvitesCreated = _streamingStorage.ReturnTeamInviteCreations();

                foreach(StreamTeamInviteCreationsResponse teamInvite in teamInvitesCreated)
                {
                    Log.Information($"sending team invite: {teamInvite.TeamInviteCreationId}");
                    await responseStream.WriteAsync(teamInvite);

                }

                await Task.Delay(250, context.CancellationToken);

            }
        }

        public override async Task StreamTeamInviteRejections(StreamTeamInviteDeletionRequest request, IServerStreamWriter<StreamTeamInviteDeletionResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                ImmutableList<StreamTeamInviteDeletionResponse> teamInviteDeletions = _streamingStorage.ReturnTeamInviteDeletions();

                foreach(StreamTeamInviteDeletionResponse teamDeletions in teamInviteDeletions)
                {
                    await responseStream.WriteAsync(teamDeletions);
                }

                _streamingStorage.ClearTeamInviteDeletion();

                await Task.Delay(250, context.CancellationToken);
            }
        }

        public override async Task StreamTeamInviteUpdate(StreamTeamInviteUpdateRequest request, IServerStreamWriter<StreamTeamInviteUpdateResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                ImmutableList<StreamTeamInviteUpdateResponse> teamInviteUpdates = _streamingStorage.ReturnTeamInviteUpdates();

                foreach(StreamTeamInviteUpdateResponse teamInviteUpdate in teamInviteUpdates)
                {

                    Log.Information($"Sending {teamInviteUpdate.TeamInviteId} : {teamInviteUpdate.InviteStatus} to be updated");
                    await responseStream.WriteAsync(teamInviteUpdate);
                }

                _streamingStorage.ClearTeamInviteUpdates();

                await Task.Delay(250, context.CancellationToken);
            }
        }

    }
}
