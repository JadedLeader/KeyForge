using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using System.Runtime.CompilerServices;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.BackgroundConsumers.TeamConsumer
{
    public class CreateTeam : GenericGrpcConsumer<StreamTeamCreationResponse, TeamDataModel>
    {
        private readonly Team.TeamClient _teamClient;

        private readonly HashSet<string> _seenTeamCreations = new();
        public CreateTeam(Team.TeamClient teamClient, IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
          _teamClient = teamClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, TeamDataModel model)
        {
            var scope = service.GetRequiredService<ITeamRepo>();

            await scope.AddAsync(model);
        }

        protected override TeamDataModel MapToType(StreamTeamCreationResponse responseType)
        {
            if (_seenTeamCreations.Contains(responseType.TeamCreationId))
            {
                return null;
            }

            _seenTeamCreations.Add(responseType.TeamCreationId);

            return MapStreamToTeam(responseType);
        }

        protected override IAsyncEnumerable<StreamTeamCreationResponse> OpenStream()
        {
            var client = _teamClient.StreamTeamCreations(new StreamTeamCreationRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        private TeamDataModel MapStreamToTeam(StreamTeamCreationResponse teamCreation)
        {
            TeamDataModel newTeam = new TeamDataModel
            {
                AccountId = Guid.Parse(teamCreation.AccountId),
                CreatedAt = DateTime.Parse(teamCreation.CreatedAt),
                CreatedBy = teamCreation.CreatedBy,
                MemberCap = teamCreation.MemberCap,
                TeamAcceptingInvites = teamCreation.TeamAcceptingInvites,
                Id = Guid.Parse(teamCreation.TeamId),
                TeamName = teamCreation.TeamName,
            };

            return newTeam;
        }

    }
}
