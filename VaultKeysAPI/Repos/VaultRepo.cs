using KeyForgedShared.Generics;
using VaultKeysAPI.DataContext;
using KeyForgedShared.SharedDataModels;
using VaultKeysAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VaultKeysAPI.Repos
{
    public class VaultRepo : GenericRepository<VaultDataModel>, IVaultRepo
    {

        private readonly VaultKeysDataContext _vaultKeysDataContext;

        public VaultRepo(VaultKeysDataContext vaultKeysDataContext) : base(vaultKeysDataContext) 
        {
            _vaultKeysDataContext = vaultKeysDataContext;
        }


        public override async Task<VaultDataModel> AddAsync(VaultDataModel databaseModel)
        {

            bool entityExists = await _vaultKeysDataContext.Vault.AnyAsync(x => x.VaultId == databaseModel.VaultId);

            if (entityExists)
            {
                return databaseModel;
            }

            return await base.AddAsync(databaseModel);
        }

        public async Task<VaultDataModel> GetVaultByUserId(Guid userId)
        {

            VaultDataModel? returningVault = await _vaultKeysDataContext.Vault.FirstOrDefaultAsync(x => x.AccountId == userId); 

            if(returningVault == null)
            {
                return null;
            }

            return returningVault;

        }

        public async Task<bool> HasVault(Guid userId, Guid vaultId)
        {
            bool userHasVaults = await _vaultKeysDataContext.Vault.AnyAsync(x => x.AccountId == userId && x.VaultId == vaultId);

            if (!userHasVaults)
            {
                return false;
            }

            return true;
        }

        public async Task<VaultDataModel> GetVaultByVaultId(Guid vaultId)
        {

            VaultDataModel? getVault = await _vaultKeysDataContext.Vault.Where(x => x.VaultId == vaultId).FirstOrDefaultAsync();

            if(getVault == null)
            {
                return null;
            }

            return getVault;
        
        }

    }
}
