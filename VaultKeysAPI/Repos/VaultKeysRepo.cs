using KeyForgedShared.Generics;
using VaultKeysAPI.DataContext;
using KeyForgedShared.SharedDataModels;
using VaultKeysAPI.Interfaces;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using KeyForgedShared.DTO_s.VaultKeysDTO_s;
using System.Security.Cryptography.Xml;
using KeyForgedShared.Projections.VaultKeysProjections;

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

        public async Task<List<GetAllVaultsDto>> ReturnVaultKeys(Guid accountId)
        {
            

            var result = await _vaultKeysDataContext.Vault.Where(x => x.AccountId == accountId)
                .Include(x => x.VaultKeys)
                .Select(x => new GetAllVaultsDto
                {
                    VaultCreatedAt = x.VaultCreatedAt, 
                    VaultId = x.VaultId,
                    VaultName = x.VaultName,
                    VaultType = (KeyForgedShared.ReturnTypes.Vaults.VaultType)x.VaultType, 
                    Keys = x.VaultKeys.Select(k => new VaultKeyDto 
                    {  

                        VaultKeyId = k.VaultKeyId,
                        DateTimeVaultKeyCreated = k.DateTimeVaultKeyCreated, 
                        HashedVaultKey = k.HashedVaultKey,
                        KeyName = k.KeyName,
                    }).ToList()
                }).ToListAsync();


            return result;


        }

        public async Task<SingleVaultWithSingleKeyProjection> ReturnVaultAndKey(Guid vaultId, Guid accountId)
        {
            SingleVaultWithSingleKeyProjection? result = await _vaultKeysDataContext.VaultKeys.Where(x => x.Vault.AccountId == accountId && x.VaultId == vaultId)
                .Select(x => new SingleVaultWithSingleKeyProjection
                {
                    VaultName =  x.Vault.VaultName, 
                    singleVaultKey = new SingleVaultKeyProjection
                    {
                        EncryptedVaultKey = x.HashedVaultKey, 
                        KeyName = x.KeyName,
                    }

                }).FirstOrDefaultAsync();

            if(result == null)
            {
                return null;
            }

            return result;
                
                
        }
        
        public async Task<List<VaultKeysDataModel>> GetVaultKeysViaVaultId(Guid vaultId)
        {

            List<VaultKeysDataModel> getVaultkeysViaVaultId = await _vaultKeysDataContext.VaultKeys.Where(x => x.VaultId == vaultId).ToListAsync();

            if(getVaultkeysViaVaultId.Count == 0)
            {
                return null;
            }

            return getVaultkeysViaVaultId;

        }




    }
}
