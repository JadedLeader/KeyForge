using VaultAPI.DataModel;

namespace VaultAPI.Interfaces.RepoInterfaces
{
    public interface IVaultRepo
    {
        Task<VaultDataModel> AddAsync(VaultDataModel databaseModel);
        Task<VaultDataModel> DeleteAsync(VaultDataModel databaseModel);
        Task<VaultDataModel> UpdateAsync(VaultDataModel databaseModel);
    }
}