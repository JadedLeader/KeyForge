using KeyForgedShared.DTO_s.TeamVaultDTO_s;
using KeyForgedShared.ReturnTypes.TeamVault;

namespace TeamVaultAPI.Interfaces.Services
{
    public interface ITeamVaultService
    {

        public Task<CreateTeamVaultReturn> CreateTeamVault(CreateTeamVaultDto createTeamVault, string shortLivedToken);

    }
}
