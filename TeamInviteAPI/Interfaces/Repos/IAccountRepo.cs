using KeyForgedShared.Interfaces;
using KeyForgedShared.SharedDataModels;

namespace TeamInviteAPI.Interfaces.Repos
{
    public interface IAccountRepo
    {
        Task<AccountDataModel> AddAsync(AccountDataModel databaseModel);
        Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel);
        Task<T> DeleteRecordViaId<T>(Guid id) where T : IEntity;
        Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T : IEntity;
        Task<T?> FindSingleRecordViaId<T>(Guid id) where T : IEntity;
        Task<bool> HasModel<T>(Guid id) where T : IEntity;
        Task<AccountDataModel> UpdateAsync(AccountDataModel databaseModel);
    }
}