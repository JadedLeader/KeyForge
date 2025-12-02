using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.BackgroundConsumers.TeamInvites
{
    public class CreateTeamInvite : GenericGrpcConsumer<StreamTeamInviteCreationsResponse, TeamInviteDataModel>
    {

        private readonly TeamInvite.TeamInviteClient _teamInviteClient;

        public CreateTeamInvite(TeamInvite.TeamInviteClient teamInviteClient, IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
            _teamInviteClient = teamInviteClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, TeamInviteDataModel model)
        {
            var teamInviteRepo = service.GetRequiredService<ITeamInviteRepo>();

            await teamInviteRepo.AddAsync(model);
        }

        protected override TeamInviteDataModel MapToType(StreamTeamInviteCreationsResponse responseType)
        {
            return MapStreamToModel(responseType);
        }

        protected override IAsyncEnumerable<StreamTeamInviteCreationsResponse> OpenStream()
        {
            var client = _teamInviteClient.StreamTeamInviteCreations(new StreamTeamInviteCreationsRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        private TeamInviteDataModel MapStreamToModel(StreamTeamInviteCreationsResponse teamInviteCreation)
        {
            TeamInviteDataModel teamInvite = new TeamInviteDataModel
            {
                Id = Guid.Parse(teamInviteCreation.TeamInviteId),
                AccountId = Guid.Parse(teamInviteCreation.AccountId),
                TeamVaultId = Guid.Parse(teamInviteCreation.TeamVaultId),
                InviteSentBy = teamInviteCreation.InviteSentBy,
                InviteRecipient = teamInviteCreation.InviteRecipient,
                InviteStatus = teamInviteCreation.InviteStatus,
                InviteCreatedAt = teamInviteCreation.InviteCreatedAt
            };

            return teamInvite;
        }
    }
}
