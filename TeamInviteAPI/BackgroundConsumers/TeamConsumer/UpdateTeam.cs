using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using System.Threading.Tasks;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.BackgroundConsumers.TeamConsumer
{
    public class UpdateTeam : GenericGrpcConsumer<StreamTeamUpdateResponse, TeamDataModel>
    {
        private readonly Team.TeamClient _teamClient;
        public UpdateTeam(Team.TeamClient teamClient, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            _teamClient = teamClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, TeamDataModel model)
        {
            var teamRepo = service.GetRequiredService<ITeamRepo>();

            await teamRepo.UpdateAsync(model);
        }

        protected override TeamDataModel MapToType(StreamTeamUpdateResponse responseType)
        {
            return MapUpdateStreamToTeam(responseType);
        }

        protected override IAsyncEnumerable<StreamTeamUpdateResponse> OpenStream()
        {
            var client = _teamClient.StreamTeamUpdates(new StreamTeamUpdateRequest()); 

            return client.ResponseStream.ReadAllAsync();
        }

        private TeamDataModel MapUpdateStreamToTeam(StreamTeamUpdateResponse streamUpdate)
        {

            TeamDataModel newTeam = new TeamDataModel
            {
                Id = Guid.Parse(streamUpdate.TeamId),
                TeamName = streamUpdate.TeamName,
                TeamAcceptingInvites = streamUpdate.TeamAcceptingInvites,
                MemberCap = streamUpdate.MemberCap,
            };

            return newTeam;

        }
    }
}
