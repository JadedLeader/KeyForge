using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamInviteAPI.DataContext;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.Repos
{
    public class TeamInviteRepo : GenericRepository<TeamInviteDataModel>, ITeamInviteRepo
    {

        private readonly TeamInviteDataContext _teamInviteRepo;

        public TeamInviteRepo(TeamInviteDataContext teamInviteRepo) : base(teamInviteRepo)
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

        public override Task<TeamInviteDataModel> UpdateAsync(TeamInviteDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }
    }
}
