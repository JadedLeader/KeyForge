using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamMembersAPI.DataContext;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.Repos
{
    public class TeamVaultRepo : GenericRepository<TeamVaultDataModel>, ITeamVaultRepo
    {
        private readonly TeamMemberDataContext _teamVaultRepo;

        public TeamVaultRepo(TeamMemberDataContext teamVaultRepo) : base(teamVaultRepo)
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

        public override Task<T> DeleteRecordViaId<T>(Guid id)
        {
            return base.DeleteRecordViaId<T>(id);
        }

        public override Task<List<T>> FindAllRecordsViaId<T>(Guid id)
        {
            return base.FindAllRecordsViaId<T>(id);
        }

        public override Task<T?> FindSingleRecordViaId<T>(Guid id) where T : class
        {
            return base.FindSingleRecordViaId<T>(id);
        }

        public override Task<bool> HasModel<T>(Guid id)
        {
            return base.HasModel<T>(id);
        }

        public override Task<TeamVaultDataModel> UpdateAsync(TeamVaultDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }
    }
}
