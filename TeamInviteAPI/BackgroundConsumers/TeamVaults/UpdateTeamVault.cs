using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.BackgroundConsumers.TeamVaults
{
    public class UpdateTeamVault : GenericGrpcConsumer<StreamTeamVaultUpdatesResponse, TeamVaultDataModel>
    {

        private readonly TeamVault.TeamVaultClient _teamVaultClient;

        public UpdateTeamVault(TeamVault.TeamVaultClient teamVaultClient, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory) 
        {
            _teamVaultClient = teamVaultClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, TeamVaultDataModel model)
        {
            var teamVaultRepo = service.GetRequiredService<ITeamVaultRepo>();

            await teamVaultRepo.UpdateAsync(model);
        }

        protected override TeamVaultDataModel MapToType(StreamTeamVaultUpdatesResponse responseType)
        {
            return MapStreamTeamVaultUpdates(responseType);
        }

        protected override IAsyncEnumerable<StreamTeamVaultUpdatesResponse> OpenStream()
        {
            var client = _teamVaultClient.StreamTeamVaultUpdates(new StreamTeamVaultUpdatesRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        private TeamVaultDataModel MapStreamTeamVaultUpdates(StreamTeamVaultUpdatesResponse teamVaultUpdates)
        {
            TeamVaultDataModel teamVault = new TeamVaultDataModel
            {
                Id = Guid.Parse(teamVaultUpdates.TeamVaultId),
                TeamVaultDescription = teamVaultUpdates.TeamVaultDescription,
                TeamVaultName = teamVaultUpdates.TeamVaultName,
                LastTeamUpdate = teamVaultUpdates.LastTeamUpdate,
                CurrentStatus = teamVaultUpdates.CurrentStatus,

            };

            return teamVault;
        }
    }
}
