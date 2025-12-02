using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.BackgroundConsumers.TeamInvites
{
    public class DeleteTeamInvite : GenericGrpcConsumer<StreamTeamInviteDeletionResponse, TeamInviteDataModel>
    {

        private readonly TeamInvite.TeamInviteClient _teamInviteClient;

        public DeleteTeamInvite(TeamInvite.TeamInviteClient teamInviteClient, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            _teamInviteClient = teamInviteClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, TeamInviteDataModel model)
        {
            var teamInviteRepo = service.GetRequiredService<ITeamInviteRepo>();

            await teamInviteRepo.DeleteAsync(model);
        }

        protected override TeamInviteDataModel MapToType(StreamTeamInviteDeletionResponse responseType)
        {
            return new TeamInviteDataModel { Id = Guid.Parse( responseType.TeamInviteId) };
        }

        protected override IAsyncEnumerable<StreamTeamInviteDeletionResponse> OpenStream()
        {
            var client = _teamInviteClient.StreamTeamInviteRejections(new StreamTeamInviteDeletionRequest());

            return client.ResponseStream.ReadAllAsync();
        }
    }
}
