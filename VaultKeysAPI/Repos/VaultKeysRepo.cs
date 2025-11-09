using KeyForgedShared.Generics;
using VaultKeysAPI.DataContext;
using KeyForgedShared.SharedDataModels;
using VaultKeysAPI.Interfaces;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

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

    }
}
