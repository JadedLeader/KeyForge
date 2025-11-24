using KeyForgedShared.SharedDataModels;

namespace TeamAPI.Interfaces.Repos
{
    public interface IAccountRepo
    {

        public Task<AccountDataModel> AddAsync(AccountDataModel databaseModel);


        public Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel);


        public Task<AccountDataModel> UpdateAsync(AccountDataModel databaseModel);
        public Task<bool> hasAccount(Guid accountId);




    }
}
