using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamMembersAPI.DataContext;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.Repos
{
    public class TeamInviteRepo : GenericRepository<TeamInviteDataModel>, ITeamInviteRepo
    {

        private readonly TeamMemberDataContext _teamInviteRepo;

        public TeamInviteRepo(TeamMemberDataContext teamInviteRepo) : base(teamInviteRepo)
        {
            _teamInviteRepo = teamInviteRepo;
        }

        public override Task<TeamInviteDataModel> AddAsync(TeamInviteDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<TeamInviteDataModel> DeleteAsync(TeamInviteDataModel databaseModel)
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

        public override Task<TeamInviteDataModel> UpdateAsync(TeamInviteDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }
    }
}
