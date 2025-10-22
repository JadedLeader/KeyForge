using KeyForgedShared.Generics;
using VaultKeysAPI.DataContext;
using VaultKeysAPI.DataModel;
using VaultKeysAPI.Interfaces;

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

    }
}
