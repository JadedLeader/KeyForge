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
        

        public async Task<VaultKeysDataModel> RemoveVaultKeyViaKeyId(Guid vaultKeyId, Guid vaultId)
        {

            VaultKeysDataModel? removeVaultKey = await _vaultKeysDataContext.VaultKeys.FirstOrDefaultAsync(x => x.VaultKeyId == vaultKeyId && x.VaultId == vaultId);

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

        public async Task<bool> HasVaultKeys(Guid vaultId)
        {

            List<VaultKeysDataModel> listOfVaultKeys = await _vaultKeysDataContext.VaultKeys.Where(x => x.VaultId == vaultId).ToListAsync();

            if(listOfVaultKeys.Count == 0)
            {
                return false;
            }

            return true;

        }

        public async Task<GetSingleVaultWithAllKeysAndDetailsProjection> GetAllDetailsForVault(Guid vaultId)
        {
            GetSingleVaultWithAllKeysAndDetailsProjection? singleVaultAllDetails = await _vaultKeysDataContext.Vault.Where(v => v.VaultId == vaultId)
                .Include(v => v.VaultKeys)
                .Select(v => new GetSingleVaultWithAllKeysAndDetailsProjection
                {
                    VaultDataModelWithAllKeys = new VaultDataModel
                    {
                        VaultId = v.VaultId,
                        AccountId = v.AccountId,
                        VaultCreatedAt = v.VaultCreatedAt,
                        VaultName = v.VaultName,
                        VaultType = v.VaultType,
                        VaultKeys = v.VaultKeys.ToList(),
                    }

                }).FirstOrDefaultAsync();

            if(singleVaultAllDetails == null)
            {
                return null;
            }

            return singleVaultAllDetails;
        }

        public async Task<List<Guid>> RemoveAllVaultsKeysFromVault(Guid vaultId)
        {
            List<VaultKeysDataModel> vaultKeys = await _vaultKeysDataContext.VaultKeys.Where(x => x.VaultId == vaultId).ToListAsync();

            if(vaultKeys.Count == 0)
            {
                return null;
            }

            List<Guid> keysRemoved = new List<Guid>();
            
            foreach(VaultKeysDataModel keys in vaultKeys)
            {

                keysRemoved.Add(keys.VaultId);

                await DeleteAsync(keys);
            }

            return keysRemoved;

        }

        public async Task<VaultDataModel> RemoveVault(Guid vaultId)
        {

            VaultDataModel? removedVault = await _vaultKeysDataContext.Vault.Where(x => x.VaultId == vaultId).FirstOrDefaultAsync();

            if(removedVault == null)
            {
                return null;
            }

            _vaultKeysDataContext.Remove(removedVault);

            await _vaultKeysDataContext.SaveChangesAsync();

            return removedVault;



        }

        public async Task<VaultKeysDataModel> GetAndUpdateVaultKeys(Guid vaultKeysId, string newEncryedKey, string newKeyName)
        {
            VaultKeysDataModel? getVaultKeys = await _vaultKeysDataContext.VaultKeys.Where(x => x.VaultKeyId == vaultKeysId).FirstOrDefaultAsync();

            if(getVaultKeys == null)
            {
                return null;
            }

            getVaultKeys.HashedVaultKey = newEncryedKey;
            getVaultKeys.KeyName = newKeyName;

            await UpdateAsync(getVaultKeys);

            return getVaultKeys;
        }




    }
}
