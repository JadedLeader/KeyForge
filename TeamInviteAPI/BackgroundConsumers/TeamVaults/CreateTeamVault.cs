using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Serilog;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.BackgroundConsumers.TeamVaults
{
    public class CreateTeamVault : GenericGrpcConsumer<StreamTeamVaultCreationResponse, TeamVaultDataModel>
    {

        private readonly TeamVault.TeamVaultClient _teamVaultClient;

        public CreateTeamVault(TeamVault.TeamVaultClient teamVaultClient, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            _teamVaultClient = teamVaultClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, TeamVaultDataModel model)
        {
            Log.Information($"Received {model.TeamVaultName} to be created");

            var teamVaultRepo = service.GetRequiredService<ITeamVaultRepo>();

            await teamVaultRepo.AddAsync(model);
        }

        protected override TeamVaultDataModel MapToType(StreamTeamVaultCreationResponse responseType)
        {
            return MapCreateTeamStreamToTeamVault(responseType);
        }

        protected override IAsyncEnumerable<StreamTeamVaultCreationResponse> OpenStream()
        {
            var client = _teamVaultClient.StreamTeamVaultCreations(new StreamTeamVaultCreationRequest());

            return client.ResponseStream.ReadAllAsync();
        }

        private TeamVaultDataModel MapCreateTeamStreamToTeamVault(StreamTeamVaultCreationResponse teamVaultCreation)
        {
            TeamVaultDataModel teamVault = new TeamVaultDataModel
            {
                Id = Guid.Parse(teamVaultCreation.TeamVaultId),
                TeamId = Guid.Parse(teamVaultCreation.TeamId),
                TeamVaultDescription = teamVaultCreation.TeamVaultDescription,
                TeamVaultName = teamVaultCreation.TeamVaultName,
                CreatedAt = teamVaultCreation.CreatedAt,
                LastTeamUpdate = teamVaultCreation.LastTeamUpdate,
                CurrentStatus = teamVaultCreation.CurrentStatus,
            }; 

            return teamVault;
        }
    }
}
