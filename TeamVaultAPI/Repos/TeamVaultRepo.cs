using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
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

    }
}
