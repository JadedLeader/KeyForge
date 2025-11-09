using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.Interfaces
{
    public interface IVaultKeysRepo
    {

        public Task<VaultKeysDataModel> AddAsync(VaultKeysDataModel databaseModel);

    }
}
