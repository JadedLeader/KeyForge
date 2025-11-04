using KeyForgedShared.Generics;
using VaultKeysAPI.DataContext;
using KeyForgedShared.SharedDataModels;
using VaultKeysAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VaultKeysAPI.Repos
{
    public class AccountRepo : GenericRepository<AccountDataModel>, IAccountRepo
    {
        private readonly VaultKeysDataContext _vaultKeysDataContext;

        public AccountRepo(VaultKeysDataContext vaultKeysDataContext) : base(vaultKeysDataContext)
        {
              _vaultKeysDataContext = vaultKeysDataContext;
        }

        public override Task<AccountDataModel> AddAsync(AccountDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public async Task<AccountDataModel> DeleteAccountViaAccountId(Guid accountId)
        {
            AccountDataModel? account = await _vaultKeysDataContext.Account.Where(ac => ac.AccountId == accountId).FirstOrDefaultAsync();

            if (account == null)
            {
                return new AccountDataModel();
            }

            await DeleteAsync(account);

            return account;
        }

    }
}
