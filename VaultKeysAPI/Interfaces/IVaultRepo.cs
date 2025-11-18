using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.Interfaces
{
    public interface IVaultRepo
    {

        public Task<VaultDataModel> AddAsync(VaultDataModel databaseModel);

        public Task<VaultDataModel> GetVaultByUserId(Guid userId);

        public Task<bool> HasVault(Guid userId, Guid vaultId);

        public Task<VaultDataModel> GetVaultByVaultId(Guid vaultId);

        public Task<VaultDataModel> DeleteVaultViaVaultId(Guid vaultId);

        public Task<VaultDataModel> CascadeDeleteIntoVaultKeys(Guid vaultId);

    }
}
