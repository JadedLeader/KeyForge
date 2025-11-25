using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamVaultAPI.DataContext;
using TeamVaultAPI.Interfaces.Repos;

namespace TeamVaultAPI.Repos
{
    public class TeamRepo : GenericRepository<TeamDataModel>, ITeamRepo
    {

        private readonly TeamVaultDataContext _dbContext;

        public TeamRepo(TeamVaultDataContext dbContext) : base(dbContext)
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

    }
}
