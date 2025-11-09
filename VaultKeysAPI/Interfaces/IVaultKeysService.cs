using KeyForgedShared.DTO_s.VaultKeysDTO_s;
using KeyForgedShared.ReturnTypes.VaultKeys;

namespace VaultKeysAPI.Interfaces
{
    public interface IVaultKeysService
    {

        public Task<AddVaultKeyReturn> AddVaultKey(AddVaultKeyDto addVaultKey, string shortLivedToken);

        public Task<RemoveVaultKeyReturn> RemoveVaultKey(RemoveVaultKeyDto removeVaultKey, string shortLivedToken);

        public Task<UpdateVaultKeyReturn> UpdateVaultKey();

        public Task<UnhashVaultKeyReturn> UnhashVaultKey();
        

    }
}
