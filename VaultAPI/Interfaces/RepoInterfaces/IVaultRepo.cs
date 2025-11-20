using KeyForgedShared.SharedDataModels;

namespace VaultAPI.Interfaces.RepoInterfaces
{
    public interface IVaultRepo
    {
        Task<VaultDataModel> AddAsync(VaultDataModel databaseModel);
        Task<VaultDataModel> DeleteAsync(VaultDataModel databaseModel);
        Task<VaultDataModel> UpdateAsync(VaultDataModel databaseModel);
        Task<VaultDataModel> GetVaultViaVaultId(Guid vaultId);
        Task UpdateVaultName(VaultDataModel vaultDataModel, string newVaultName);
        Task<List<VaultDataModel>> GetVaultsViaAccountId(Guid accountId);

        public Task DeleteAllVaultsForAccount(Guid accountId);
    }
}