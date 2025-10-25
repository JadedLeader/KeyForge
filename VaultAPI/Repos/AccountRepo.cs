using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;
using VaultAPI.DataContext;
using KeyForgedShared.Generics;
using VaultAPI.Interfaces.RepoInterfaces;

namespace VaultAPI.Repos
{
    public class AccountRepo : GenericRepository<AccountDataModel>, IAccountRepo
    {

        private readonly VaultDataContext _dataContext;
        public AccountRepo(VaultDataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public override Task<AccountDataModel> AddAsync(AccountDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public override Task<AccountDataModel> UpdateAsync(AccountDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }

        public async Task<AccountDataModel> DeleteAccountViaAccountId(Guid accountId)
        {
            AccountDataModel? account = await _dataContext.Account.Where(ac => ac.AccountId == accountId).FirstOrDefaultAsync();

            if (account == null)
            {
                return new AccountDataModel();
            }

            await DeleteAsync(account);

            return account;
        }

        public async Task<AccountDataModel> CheckForExistingAccount(Guid accountId)
        {
            AccountDataModel? getAccount = await _dataContext.Account.Where(ac => ac.AccountId == accountId).FirstOrDefaultAsync();

            if(getAccount == null)
            {
                return new AccountDataModel();
            }

            return getAccount;
        }





    }
}
