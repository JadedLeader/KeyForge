using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.Interfaces
{
    public interface IVaultKeysRepo
    {

        public Task<VaultKeysDataModel> AddAsync(VaultKeysDataModel databaseModel);

        public Task<VaultKeysDataModel> UpdateAsync(VaultKeysDataModel databaseModel);

        public Task<VaultKeysDataModel> DeleteAsync(VaultKeysDataModel databaseModel);

        public Task<VaultKeysDataModel> RemoveVaultKeyViaKeyId(Guid vaultKeyId);

    }
}
