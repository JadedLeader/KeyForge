using KeyForgedShared.SharedDataModels;

namespace TeamVaultAPI.Interfaces.Repos
{
    public interface ITeamRepo
    {

        public Task<TeamDataModel> AddAsync(TeamDataModel databaseModel);

        public Task<TeamDataModel> DeleteAsync(TeamDataModel databaseModel);

        public Task<TeamDataModel> UpdateAsync(TeamDataModel databaseModel);
        

    }
}
