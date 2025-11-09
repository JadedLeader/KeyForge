using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.Mappings
{
    public class VaultKeysMappings
    {


        public VaultKeysDataModel CreateVaultKeysDataModel(string hashedVaultKey, VaultDataModel vault)
        {
            VaultKeysDataModel newVaultKeys = new VaultKeysDataModel
            {
                VaultKeyId = Guid.NewGuid(),
                VaultId = vault.VaultId,
                HashedVaultKey = hashedVaultKey,
                Vault = vault,
                DateTimeVaultKeyCreated = DateTime.Now.ToString(),
                
            };

            return newVaultKeys;

        }

    }
}
