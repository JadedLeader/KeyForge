using KeyForgedShared.Generics;
using VaultKeysAPI.DataContext;
using VaultKeysAPI.DataModel;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.Repos
{
    public class AuthRepo : GenericRepository<AuthDataModel>, IAuthRepo
    {
        private readonly VaultKeysDataContext _vaultKeysDataContext;

        public AuthRepo(VaultKeysDataContext vaultKeysDataContext) : base(vaultKeysDataContext)
        {
            _vaultKeysDataContext = vaultKeysDataContext;
        }

        public override Task<AuthDataModel> DeleteAsync(AuthDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }
    }
}
