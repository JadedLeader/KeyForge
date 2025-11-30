using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamInviteAPI.DataContext;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.Repos
{
    public class TeamVaultRepo : GenericRepository<TeamVaultDataModel>, ITeamVaultRepo
    {

        private readonly TeamInviteDataContext _teamVaultRepo;

        public TeamVaultRepo(TeamInviteDataContext teamVaultRepo) : base(teamVaultRepo)
        {
            _teamVaultRepo = teamVaultRepo;
        }

        public override Task<TeamVaultDataModel> AddAsync(TeamVaultDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<TeamVaultDataModel> DeleteAsync(TeamVaultDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public override Task<T> DeleteRecordViaId<T>(Guid id) where T : class
        {
            return base.DeleteRecordViaId<T>(id);
        }

        public override Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T : class
        {
            return base.FindAllRecordsViaId<T>(id);
        }

        public override Task<T?> FindSingleRecordViaId<T>(Guid id) where T : class
        {
            return base.FindSingleRecordViaId<T>(id);
        }

        public override Task<bool> HasModel<T>(Guid id) where T : class
        {
            return base.HasModel<T>(id);
        }

        public override Task<TeamVaultDataModel> UpdateAsync(TeamVaultDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }
    }
}
