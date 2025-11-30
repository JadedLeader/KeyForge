using KeyForgedShared.Interfaces;
using KeyForgedShared.SharedDataModels;

namespace TeamInviteAPI.Interfaces.Repos
{
    public interface ITeamVaultRepo
    {
        Task<TeamVaultDataModel> AddAsync(TeamVaultDataModel databaseModel);
        Task<TeamVaultDataModel> DeleteAsync(TeamVaultDataModel databaseModel);
        Task<T> DeleteRecordViaId<T>(Guid id) where T : IEntity;
        Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T : IEntity;
        Task<T?> FindSingleRecordViaId<T>(Guid id) where T : IEntity;
        Task<bool> HasModel<T>(Guid id) where T : IEntity;
        Task<TeamVaultDataModel> UpdateAsync(TeamVaultDataModel databaseModel);
    }
}