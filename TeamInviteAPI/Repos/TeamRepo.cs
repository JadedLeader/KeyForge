using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;
using TeamInviteAPI.DataContext;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.Repos
{
    public class TeamRepo : GenericRepository<TeamDataModel>, ITeamRepo
    {

        private readonly TeamInviteDataContext _teamRepo;

        public TeamRepo(TeamInviteDataContext teamRepo) : base(teamRepo)
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

        public override Task<TeamDataModel> UpdateAsync(TeamDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }

        public async Task<bool> HasTeamVaultAndTeamInvitesOpen(Guid teamVaultId)
        {
            TeamDataModel? hasModelWithOpenInvites = await _teamRepo.Team.Where(x => x.TeamAcceptingInvites == "open" && x.TeamVault.Id == teamVaultId).FirstOrDefaultAsync();

            if(hasModelWithOpenInvites == null)
            {
                return false;
            }

            return true;

        }

    }
}
