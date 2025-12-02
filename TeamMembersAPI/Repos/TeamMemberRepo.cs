using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamMembersAPI.DataContext;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.Repos
{
    public class TeamMemberRepo : GenericRepository<TeamMemberDataModel>, ITeamMemberRepo
    {

        private readonly TeamMemberDataContext _teamMemberRepo;

        public TeamMemberRepo(TeamMemberDataContext teamMemberRepo) : base(teamMemberRepo)
        {
            _teamMemberRepo = teamMemberRepo;
        }

        public override Task<TeamMemberDataModel> AddAsync(TeamMemberDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<TeamMemberDataModel> DeleteAsync(TeamMemberDataModel databaseModel)
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

        public override Task<TeamMemberDataModel> UpdateAsync(TeamMemberDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }
    }
}
