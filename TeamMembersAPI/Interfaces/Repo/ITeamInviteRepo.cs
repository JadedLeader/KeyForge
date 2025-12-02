using KeyForgedShared.Interfaces;
using KeyForgedShared.SharedDataModels;

namespace TeamMembersAPI.Interfaces.Repo
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
    }
}