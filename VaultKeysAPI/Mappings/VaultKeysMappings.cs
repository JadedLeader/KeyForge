using KeyForgedShared.Projections.VaultKeysProjections;
using KeyForgedShared.ReturnTypes.VaultKeys;
using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.Mappings
{
    public class VaultKeysMappings
    {


        public VaultKeysDataModel CreateVaultKeysDataModel(string hashedVaultKey, VaultDataModel vault, string keyName)
        {
            VaultKeysDataModel newVaultKeys = new VaultKeysDataModel
            {
                VaultKeyId = Guid.NewGuid(),
                VaultId = vault.VaultId,
                HashedVaultKey = hashedVaultKey,
                Vault = vault,
                DateTimeVaultKeyCreated = DateTime.Now.ToString(),
                KeyName = keyName
                
            };

            return newVaultKeys;

        }

        public GetSingleVaultWithAllDetailsReturn MapProjectionToGetSingleVaultAllDetails(GetSingleVaultWithAllKeysAndDetailsProjection projection)
        {
            GetSingleVaultWithAllDetailsReturn newVaultWithDetails = new GetSingleVaultWithAllDetailsReturn
            {
                AccountId = projection.VaultDataModelWithAllKeys.AccountId.ToString(),
                VaultCreatedAt = projection.VaultDataModelWithAllKeys.VaultCreatedAt.ToString(),
                VaultId = projection.VaultDataModelWithAllKeys.VaultId.ToString(),
                VaultName = projection.VaultDataModelWithAllKeys.VaultName,
                VaultType = projection.VaultDataModelWithAllKeys.VaultType,
                VaultKeys = projection.VaultDataModelWithAllKeys.VaultKeys.ToList(),
                Success = true
            };

            return newVaultWithDetails;
        }

    }
}
