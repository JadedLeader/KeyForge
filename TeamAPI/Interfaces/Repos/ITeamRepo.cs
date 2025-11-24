using KeyForgedShared.SharedDataModels;

namespace TeamAPI.Interfaces.Repos
{
    public interface ITeamRepo
    {

        public Task<TeamDataModel> AddAsync(TeamDataModel databaseModel);
        public Task<TeamDataModel> DeleteAsync(TeamDataModel databaseModel);
        public Task<TeamDataModel> UpdateAsync(TeamDataModel databaseModel);
        public Task<List<TeamDataModel>> GetTeamsFromAccountId(Guid accountId);
        public Task<bool> HasTeam(Guid accountId);
        public Task<TeamDataModel> DeleteTeamViaId(Guid teamId);
        public Task<TeamDataModel> GetTeamViaId(Guid teamId);



    }
}
