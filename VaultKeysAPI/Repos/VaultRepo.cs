using KeyForgedShared.Generics;
using VaultKeysAPI.DataContext;
using VaultKeysAPI.DataModel;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.Repos
{
    public class VaultRepo : GenericRepository<VaultDataModel>, IVaultRepo
    {

        private readonly VaultKeysDataContext _vaultKeysDataContext;

        public VaultRepo(VaultKeysDataContext vaultKeysDataContext) : base(vaultKeysDataContext) 
        {
            _vaultKeysDataContext = vaultKeysDataContext;
        }

    }
}
