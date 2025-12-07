using KeyForgedShared.Enums;
using KeyForgedShared.Generics;
using KeyForgedShared.Projections.TeamInviteProjections;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;
using System.Runtime;
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

        public async Task<List<PendingTeamInvitesProjection>> GetTeamVaultPendingInvites(Guid teamVaultId)
        {

            List<PendingTeamInvitesProjection>? pendingInvites = await _teamInviteRepo.TeamInvite.Where(x => x.TeamVaultId == teamVaultId && x.InviteStatus == InviteStatus.Pending.ToString())
                .Select(x => new PendingTeamInvitesProjection
                {
                    InviteRecipient = x.InviteRecipient,
                    InviteSentBy = x.InviteSentBy,
                    InviteCreatedAt = x.InviteCreatedAt, 
                    TeamInviteId = x.Id.ToString(),
                }).ToListAsync();

            if(pendingInvites == null)
            {
                return null;
            }

            return pendingInvites;



        }

        public async Task<TeamInviteDataModel> GetTeamInviteViateamVautAndRecipient(string recipientEmail, Guid TeamVaultId)
        {

            TeamInviteDataModel? teamInviteRecord = await _teamInviteRepo.TeamInvite.Where(x => x.TeamVaultId == TeamVaultId && x.InviteRecipient == recipientEmail).FirstOrDefaultAsync();

            if(teamInviteRecord == null)
            {

                return null;
            }

            return teamInviteRecord;
        }

        public async Task<List<PendingTeamInvitesProjection>> GetPendingInvitesForAccount(string recipeintEmail)
        {
            List<PendingTeamInvitesProjection> listOfPendingInvites = await _teamInviteRepo.TeamInvite.Where(x => x.InviteRecipient == recipeintEmail && x.InviteStatus == InviteStatus.Pending.ToString())
                .Select(x => new PendingTeamInvitesProjection
                {
                    InviteSentBy = x.InviteSentBy,
                    InviteCreatedAt = x.InviteCreatedAt,
                    InviteRecipient = x.InviteRecipient,
                    TeamInviteId = x.Id.ToString(), 
                    TeamVaultId = x.TeamVaultId.ToString(),
                }).ToListAsync();

            return listOfPendingInvites;
        }

    }
}
