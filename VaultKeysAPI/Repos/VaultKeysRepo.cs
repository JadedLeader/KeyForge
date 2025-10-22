using KeyForgedShared.Generics;
using VaultKeysAPI.DataContext;
using VaultKeysAPI.DataModel;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.Repos
{
    public class VaultKeysRepo : GenericRepository<VaultKeysDataModel>, IVaultKeysRepo
    {

        private readonly VaultKeysDataContext _vaultKeysDataContext;
        public VaultKeysRepo(VaultKeysDataContext vaultKeysDataContext) : base(vaultKeysDataContext) 
        {
            _vaultKeysDataContext = vaultKeysDataContext;
        }

    }
}
