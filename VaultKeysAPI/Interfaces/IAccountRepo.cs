using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.Interfaces
{
    public interface IAccountRepo
    {
        public Task<AccountDataModel> AddAsync(AccountDataModel databaseModel);

    }
}
