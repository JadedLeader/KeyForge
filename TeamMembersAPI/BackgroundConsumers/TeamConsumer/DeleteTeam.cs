using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using System.Threading.Tasks;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.BackgroundConsumers.TeamConsumer
{
    public class DeleteTeam : GenericGrpcConsumer<StreamTeamDeletionResponse, TeamDataModel>
    {

        private readonly Team.TeamClient _teamClient;

        public DeleteTeam(Team.TeamClient teamClient, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            _teamClient = teamClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, TeamDataModel model)
        {
            var teamService = service.GetRequiredService<ITeamRepo>();

            await teamService.DeleteAsync(model);
        }

        protected override TeamDataModel MapToType(StreamTeamDeletionResponse responseType)
        {
            return MapToTeam(responseType);
        }

        protected override IAsyncEnumerable<StreamTeamDeletionResponse> OpenStream()
        {
            var client = _teamClient.StreamTeamDeletions(new StreamTeamDeletionRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        private TeamDataModel MapToTeam(StreamTeamDeletionResponse teamDeletionResponse)
        {
            TeamDataModel teamDataModel = new TeamDataModel
            {
                Id = Guid.Parse(teamDeletionResponse.TeamId),
            };

            return teamDataModel;
        }
    }
}
