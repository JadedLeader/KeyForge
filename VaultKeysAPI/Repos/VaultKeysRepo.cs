using KeyForgedShared.Generics;
using VaultKeysAPI.DataContext;
using KeyForgedShared.SharedDataModels;
using VaultKeysAPI.Interfaces;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace VaultKeysAPI.Repos
{
    public class VaultKeysRepo : GenericRepository<VaultKeysDataModel>, IVaultKeysRepo
    {

        private readonly VaultKeysDataContext _vaultKeysDataContext;
        public VaultKeysRepo(VaultKeysDataContext vaultKeysDataContext) : base(vaultKeysDataContext) 
        {
            _vaultKeysDataContext = vaultKeysDataContext;
        }


        public override async Task<VaultKeysDataModel> AddAsync(VaultKeysDataModel databaseModel)
        {

            bool vaultKeyExists = await _vaultKeysDataContext.VaultKeys.AnyAsync(x => x.VaultKeyId == databaseModel.VaultKeyId);

            if (vaultKeyExists)
            {
                return databaseModel;
            }

            return await base.AddAsync(databaseModel);
        }

        public override Task<VaultKeysDataModel> UpdateAsync(VaultKeysDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }

        public override Task<VaultKeysDataModel> DeleteAsync(VaultKeysDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }
        

        public async Task<VaultKeysDataModel> RemoveVaultKeyViaKeyId(Guid vaultKeyId)
        {

            VaultKeysDataModel? removeVaultKey = await _vaultKeysDataContext.VaultKeys.FirstOrDefaultAsync(x => x.VaultKeyId == vaultKeyId);

            if (removeVaultKey == null)
            {
                return null;
            }

            return removeVaultKey;

        }

        public async Task ReturnVaultKeys(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ReturnEncryptedVaultKeyFromVaultKeyId(Guid vaultKeyId)
        {
            VaultKeysDataModel? vaultKey = await _vaultKeysDataContext.VaultKeys.FirstOrDefaultAsync(x => x.VaultKeyId == vaultKeyId);

            if(vaultKey == null)
            {
                return null;
            }

            return vaultKey.HashedVaultKey;
        }

  

    }
}
