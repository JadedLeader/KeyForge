using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;
using TeamAPI.DataContext;
using TeamAPI.Interfaces.Repos;
using TeamAPI.Services;

namespace TeamAPI.Repos
{
    public class TeamRepo : GenericRepository<TeamDataModel>, ITeamRepo
    {

        private readonly TeamApiDataContext _dbContext;

        public TeamRepo(TeamApiDataContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<TeamDataModel> AddAsync(TeamDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<TeamDataModel> DeleteAsync(TeamDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public override Task<TeamDataModel> UpdateAsync(TeamDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }

        public async Task<List<TeamDataModel>> GetTeamsFromAccountId(Guid accountId)
        {

            List<TeamDataModel> teams = await _dbContext.Team.Where(a => a.AccountId == accountId).ToListAsync();  

            if(teams.Count == 0)
            {
                return null;
            }

            return teams;

        }

        public async Task<bool> HasTeam(Guid accountId)
        {
             bool team = await _dbContext.Team.AnyAsync(x => x.AccountId == accountId);

            if (!team)
            {
                return false;
            }

            return true;
        }

        public async Task<TeamDataModel> DeleteTeamViaId(Guid teamId)
        {
            TeamDataModel? team = await _dbContext.Team.Where(x => x.TeamId == teamId).FirstOrDefaultAsync();    

            if(team == null)
            {
                return null;
            }

            return team;
        }

        public async Task<TeamDataModel> GetTeamViaId(Guid teamId)
        {
            TeamDataModel? team = await _dbContext.Team.FirstOrDefaultAsync(x => x.TeamId == teamId);

            if (team == null)
            {
                return null;
            }

            return team;
        }

    }
}
