using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.Interfaces
{
    public interface IVaultRepo
    {

        public Task<VaultDataModel> AddAsync(VaultDataModel databaseModel);

        public Task<VaultDataModel> GetVaultByUserId(Guid userId);

    }
}
