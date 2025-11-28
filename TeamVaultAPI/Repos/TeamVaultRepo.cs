using KeyForgedShared.Generics;
using KeyForgedShared.Interfaces;
using KeyForgedShared.Projections.TeamVaultProjections;
using KeyForgedShared.ReturnTypes.TeamVault;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;
using TeamVaultAPI.DataContext;
using TeamVaultAPI.Interfaces.Repos;

namespace TeamVaultAPI.Repos
{
    public class TeamVaultRepo : GenericRepository<TeamVaultDataModel>, ITeamVaultRepo
    {

        private readonly TeamVaultDataContext _dbContext;

        public TeamVaultRepo(TeamVaultDataContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<TeamVaultDataModel> AddAsync(TeamVaultDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<TeamVaultDataModel> DeleteAsync(TeamVaultDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public override Task<TeamVaultDataModel> UpdateAsync(TeamVaultDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }

        public async Task<List<GetTeamWithNoVault>> GetTeamsWithNoVaults(Guid accountId)
        {
            var teamsWithNoVaults = await _dbContext.Team.Where(t => t.AccountId == accountId && t.TeamVault == null)
                .Select(x => new GetTeamWithNoVault
                {
                    Id = x.Id.ToString(),
                    TeamName = x.TeamName,  
                }).ToListAsync();

            return teamsWithNoVaults;
        }

        public override Task<bool> HasModel<T>(Guid id) where T : class
        {
            return base.HasModel<T>(id);
        }

        public override Task<T> DeleteRecordViaId<T>(Guid id) where T: class
        {
            return base.DeleteRecordViaId<T>(id);
        }

    }
}
