using AccountAPI.DataModel;
using Microsoft.EntityFrameworkCore;
using VaultAPI.DataContext;
using VaultAPI.Repos.GenericRepository;

namespace VaultAPI.Repos
{
    public class AccountRepo : GenericRepository<AccountDataModel>
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
            AccountDataModel? account =  await _dataContext.Account.Where(ac => ac.AccountId == accountId).FirstOrDefaultAsync();

            if (account == null)
            {
                return new AccountDataModel();
            }

            await DeleteAsync(account);

            return account;
        }





    }
}
