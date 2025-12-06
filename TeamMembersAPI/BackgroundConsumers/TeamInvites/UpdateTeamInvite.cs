using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.ReturnTypes.Auth;
using KeyForgedShared.SharedDataModels;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.BackgroundConsumers.TeamInvites
{
    public class UpdateTeamInvite : GenericGrpcConsumer<StreamTeamInviteUpdateResponse, TeamInviteDataModel>
    {
        private readonly TeamInvite.TeamInviteClient _client;
        public UpdateTeamInvite(TeamInvite.TeamInviteClient client, IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
            _client = client;
        }

        protected override async Task HandleMessage(IServiceProvider service, TeamInviteDataModel model)
        {
            var teamInviteRepo = service.GetRequiredService<ITeamInviteRepo>(); 

            await teamInviteRepo.UpdateAsync(model);
        }

        protected override TeamInviteDataModel MapToType(StreamTeamInviteUpdateResponse responseType)
        {
            return MapStreamUpdateToModel(responseType);
        }

        protected override IAsyncEnumerable<StreamTeamInviteUpdateResponse> OpenStream()
        {
            var client = _client.StreamTeamInviteUpdate(new StreamTeamInviteUpdateRequest()); 

            return client.ResponseStream.ReadAllAsync();
        }

        private TeamInviteDataModel MapStreamUpdateToModel(StreamTeamInviteUpdateResponse teamInviteUpdates)
        {
            TeamInviteDataModel teamInvite = new TeamInviteDataModel
            {
                Id = Guid.Parse(teamInviteUpdates.TeamInviteId),
                InviteStatus = teamInviteUpdates.InviteStatus,
            };

            return teamInvite;
        }

        
    }
}
