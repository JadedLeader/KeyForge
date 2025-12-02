using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamMembersAPI.DataContext;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.Repos
{
    public class TeamRepo : GenericRepository<TeamDataModel>, ITeamRepo
    {

        private readonly TeamMemberDataContext _teamRepo;

        public TeamRepo(TeamMemberDataContext teamRepo) : base(teamRepo)
        {
            _teamRepo = teamRepo;
        }

        public override Task<TeamDataModel> AddAsync(TeamDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<TeamDataModel> DeleteAsync(TeamDataModel databaseModel)
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

        public override Task<TeamDataModel> UpdateAsync(TeamDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }
    }
}
