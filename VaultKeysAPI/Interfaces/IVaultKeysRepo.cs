using KeyForgedShared.DTO_s.VaultKeysDTO_s;
using KeyForgedShared.Projections.VaultKeysProjections;
using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.Interfaces
{
    public interface IVaultKeysRepo
    {

        public Task<VaultKeysDataModel> AddAsync(VaultKeysDataModel databaseModel);

        public Task<VaultKeysDataModel> UpdateAsync(VaultKeysDataModel databaseModel);

        public Task<VaultKeysDataModel> DeleteAsync(VaultKeysDataModel databaseModel);

        public Task<VaultKeysDataModel> RemoveVaultKeyViaKeyId(Guid vaultKeyId, Guid vaultId);

        public Task<List<GetAllVaultsDto>> ReturnVaultKeys(Guid accountId);

        public Task<SingleVaultWithSingleKeyProjection> ReturnVaultAndKey(Guid vaultId, Guid accountId);

        public Task<List<VaultKeysDataModel>> GetVaultKeysViaVaultId(Guid vaultId);

        public Task<List<Guid>> RemoveAllVaultsKeysFromVault(Guid vaultId);

        public Task<VaultDataModel> RemoveVault(Guid vaultId);

        public Task<bool> HasVaultKeys(Guid vaultId);

        public Task<GetSingleVaultWithAllKeysAndDetailsProjection> GetAllDetailsForVault(Guid vaultId);

        public Task<VaultKeysDataModel> GetAndUpdateVaultKeys(Guid vaultKeysId, string newEncryedKey, string newKeyName);

    }
}
