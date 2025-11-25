using KeyForgedShared.SharedDataModels;

namespace TeamVaultAPI.Interfaces.Repos
{
    public interface ITeamVaultRepo
    {

        public Task<TeamVaultDataModel> AddAsync(TeamVaultDataModel databaseModel);

        public Task<TeamVaultDataModel> DeleteAsync(TeamVaultDataModel databaseModel);

        public Task<TeamVaultDataModel> UpdateAsync(TeamVaultDataModel databaseModel);
        
    }
}
