using KeyForgedShared.Interfaces;
using KeyForgedShared.Projections.TeamInviteProjections;
using KeyForgedShared.SharedDataModels;

namespace TeamInviteAPI.Interfaces.Repos
{
    public interface ITeamInviteRepo
    {
        Task<TeamInviteDataModel> AddAsync(TeamInviteDataModel databaseModel);
        Task<TeamInviteDataModel> DeleteAsync(TeamInviteDataModel databaseModel);
        Task<T> DeleteRecordViaId<T>(Guid id) where T : IEntity;
        Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T : IEntity;
        Task<T?> FindSingleRecordViaId<T>(Guid id) where T : IEntity;
        Task<bool> HasModel<T>(Guid id) where T : IEntity;
        Task<TeamInviteDataModel> UpdateAsync(TeamInviteDataModel databaseModel);

        Task<List<PendingTeamInvitesProjection>> GetTeamVaultPendingInvites(Guid teamVaultId);
        Task<TeamInviteDataModel> GetTeamInviteViateamVautAndRecipient(string recipientEmail, Guid TeamVaultId);

    }
}