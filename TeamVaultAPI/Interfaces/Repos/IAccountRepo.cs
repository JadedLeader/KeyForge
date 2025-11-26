using KeyForgedShared.Interfaces;
using KeyForgedShared.SharedDataModels;

namespace TeamVaultAPI.Interfaces.Repos
{
    public interface IAccountRepo
    {

        public Task<AccountDataModel> AddAsync(AccountDataModel databaseModel);

        public Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel);

        public Task<AccountDataModel> UpdateAsync(AccountDataModel databaseModel);

        public Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T : IEntity;

        public Task<T?> FindSingleRecordViaId<T>(Guid id) where T: IEntity;

        public Task<bool> HasModel<T>(Guid id) where T : IEntity;
       


    }
}
