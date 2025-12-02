using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamMembersAPI.Interfaces.Repo;


namespace TeamMembersAPI.BackgroundConsumers.TeamVaults
{
    public class DeleteTeamVault : GenericGrpcConsumer<StreamTeamVaultDeletionsResponse, TeamVaultDataModel>
    {

        private readonly TeamVault.TeamVaultClient _teamVaultClient;

        public DeleteTeamVault(TeamVault.TeamVaultClient teamVaultClient, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory) 
        {
            _teamVaultClient = teamVaultClient;
        }

        protected override async Task HandleMessage(IServiceProvider service, TeamVaultDataModel model)
        {
            var teamVaultRepo = service.GetRequiredService<ITeamVaultRepo>();

            await teamVaultRepo.DeleteAsync(model);
        }

        protected override TeamVaultDataModel MapToType(StreamTeamVaultDeletionsResponse responseType)
        {
            return MapStreamTeamVaultDeletion(responseType);
        }

        protected override IAsyncEnumerable<StreamTeamVaultDeletionsResponse> OpenStream()
        {
            var client = _teamVaultClient.StreamTeamVaultDeletions(new StreamTeamVaultDeletionsRequest()); 

            return client.ResponseStream.ReadAllAsync();
        }

        private TeamVaultDataModel MapStreamTeamVaultDeletion(StreamTeamVaultDeletionsResponse teamVaultDeletions)
        {
            TeamVaultDataModel teamVault = new TeamVaultDataModel
            {
                Id = Guid.Parse(teamVaultDeletions.TeamVaultId)
            };

            return teamVault;
        }
    }
}
