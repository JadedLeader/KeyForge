using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.Interfaces
{
    public interface IAccountRepo
    {
        public Task<AccountDataModel> AddAsync(AccountDataModel databaseModel);

        public Task<AccountDataModel> DeleteAccountViaAccountId(Guid accountId);

        public Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel);

    }
}
