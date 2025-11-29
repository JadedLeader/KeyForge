using KeyForgedShared.DTO_s.TeamVaultDTO_s;
using KeyForgedShared.ReturnTypes.TeamVault;

namespace TeamVaultAPI.Interfaces.Services
{
    public interface ITeamVaultService
    {

        public Task<CreateTeamVaultReturn> CreateTeamVault(CreateTeamVaultDto createTeamVault, string shortLivedToken);

        public Task<GetTeamWithNoVaultsReturn> GetTeamsWithNoVaults(string shortLivedToken);

        public Task<DeleteTeamVaultReturn> DeleteTeamVault(DeleteTeamVaultDto deleteTeamVault, string shortLivedToken);

        public Task<UpdateTeamVaultReturn> UpdateTeamVault(UpdateTeamVaultDto updateTeamVault, string shortLivedToken);

        public Task<GetTeamVaultReturn> GetTeamVault(GetTeamVaultDto getTeamVault, string shortLivedToken);

    }
}
