using KeyForgedShared.Interfaces;
using KeyForgedShared.SharedDataModels;

namespace TeamInviteAPI.Interfaces.Repos
{
    public interface ITeamRepo
    {
        Task<TeamDataModel> AddAsync(TeamDataModel databaseModel);
        Task<TeamDataModel> DeleteAsync(TeamDataModel databaseModel);
        Task<T> DeleteRecordViaId<T>(Guid id) where T : IEntity;
        Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T : IEntity;
        Task<T?> FindSingleRecordViaId<T>(Guid id) where T : IEntity;
        Task<bool> HasModel<T>(Guid id) where T : IEntity;
        Task<TeamDataModel> UpdateAsync(TeamDataModel databaseModel);

        Task<bool> HasTeamVaultAndTeamInvitesOpen(Guid teamVaultId);
    }
}