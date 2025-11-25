using KeyForgedShared.SharedDataModels;

namespace TeamVaultAPI.Interfaces.Repos
{
    public interface IAccountRepo
    {

        public Task<AccountDataModel> AddAsync(AccountDataModel databaseModel);

        public Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel);

        public Task<AccountDataModel> UpdateAsync(AccountDataModel databaseModel);
      

    }
}
