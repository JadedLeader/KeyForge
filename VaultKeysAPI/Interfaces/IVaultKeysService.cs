using KeyForgedShared.DTO_s.VaultKeysDTO_s;
using KeyForgedShared.ReturnTypes.VaultKeys;

namespace VaultKeysAPI.Interfaces
{
    public interface IVaultKeysService
    {

        public Task<AddVaultKeyReturn> AddVaultKey(AddVaultKeyDto addVaultKey, string shortLivedToken);

        public Task<RemoveVaultKeyReturn> RemoveVaultKey(RemoveVaultKeyDto removeVaultKey, string shortLivedToken);

        public Task<UpdateVaultKeyReturn> UpdateVaultKey(UpdateVaultKeyDto updateVaultKey, string shortLivedToken);

        public Task<DecryptVaultKeyReturn> DecryptVaultKey(DecryptVaultKeyDto decryptVaultkey, string shortLivedToken);

        public Task<List<GetAllVaultsDto>> ReturnAllVaultsForUser(string shortLivedToken);



    }
}
