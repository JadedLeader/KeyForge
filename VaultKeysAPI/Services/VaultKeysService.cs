using KeyForgedShared.DTO_s.VaultKeysDTO_s;
using KeyForgedShared.Interfaces;
using KeyForgedShared.ReturnTypes.VaultKeys;
using KeyForgedShared.SharedDataModels;
using VaultKeysAPI.Interfaces;
using VaultKeysAPI.Mappings;
using VaultKeysAPI.Repos;

namespace VaultKeysAPI.Services
{
    public class VaultKeysService : IVaultKeysService
    {
        private readonly IVaultKeysRepo _vaultKeysRepo;

        private readonly IVaultRepo _vaultRepo; 

        private readonly IJwtHelper _jwtHelper;

        private readonly VaultKeysMappings _vaultKeysMappings;
        
        public VaultKeysService(IVaultKeysRepo vaultKeysRepo, IJwtHelper jwtHelper, IVaultRepo vaultRepo)
        {
            _vaultKeysRepo = vaultKeysRepo;
            _jwtHelper = jwtHelper;
            _vaultRepo = vaultRepo;
        }

        public async Task<AddVaultKeyReturn> AddVaultKey(AddVaultKeyDto addVaultKey, string shortLivedToken)
        {
            AddVaultKeyReturn addVaultKeyResponse = new AddVaultKeyReturn();

            string? returnUserIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(returnUserIdFromToken == string.Empty)
            {
                addVaultKeyResponse.Success = false;
                addVaultKeyResponse.TimeOfKeyCreation = string.Empty;
                addVaultKeyResponse.VaultName = string.Empty;
                addVaultKeyResponse.HashedKey = string.Empty;
                addVaultKeyResponse.KeyName = string.Empty; 

                return addVaultKeyResponse;
            }

            VaultDataModel returningVault = await _vaultRepo.GetVaultByUserId(Guid.Parse(returnUserIdFromToken));

            if(returningVault == null)
            {
                addVaultKeyResponse.Success = false;
                addVaultKeyResponse.TimeOfKeyCreation = string.Empty;
                addVaultKeyResponse.VaultName = string.Empty;
                addVaultKeyResponse.HashedKey = string.Empty;
                addVaultKeyResponse.KeyName = string.Empty;

                return addVaultKeyResponse;
            }

            string vaultHash = HashVaultKey(addVaultKey.PasswordToHash);

            VaultKeysDataModel vaultKeys = _vaultKeysMappings.CreateVaultKeysDataModel(vaultHash, returningVault);

            await _vaultKeysRepo.AddAsync(vaultKeys);

            addVaultKeyResponse.Success = true;
            addVaultKeyResponse.TimeOfKeyCreation = vaultKeys.DateTimeVaultKeyCreated;
            addVaultKeyResponse.VaultName = returningVault.VaultName;
            addVaultKeyResponse.HashedKey = vaultKeys.HashedVaultKey;
            addVaultKeyResponse.KeyName = string.Empty;

            return addVaultKeyResponse;

        }

        public async Task<RemoveVaultKeyReturn> RemoveVaultKey(RemoveVaultKeyDto removeVaultKey, string shortLivedToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateVaultKeyReturn> UpdateVaultKey()
        {
            throw new NotImplementedException();
        }

        public async Task<UnhashVaultKeyReturn> UnhashVaultKey()
        {
            throw new NotImplementedException();
        }

        private string HashVaultKey(string keyToHash)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(keyToHash, 10);
        }
    }
}
