using CryptoNet;
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

        private readonly IConfiguration _configuration;

        private readonly ICryptoNetAes _aes;
        
        public VaultKeysService(IVaultKeysRepo vaultKeysRepo, IJwtHelper jwtHelper, IVaultRepo vaultRepo, IConfiguration config)
        {
            _vaultKeysRepo = vaultKeysRepo;
            _jwtHelper = jwtHelper;
            _vaultRepo = vaultRepo;
            _configuration = config;

            string? sharedKey = _configuration["Vault:AesKey"];

            if (string.IsNullOrEmpty(sharedKey))
            {
                throw new Exception($"AES key not found");
            }
                
            _aes = new CryptoNetAes(sharedKey);
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

            string vaultKeyEncrypted = EncryptVaultKey(addVaultKey.PasswordToEncrypt);

            VaultKeysDataModel vaultKeys = _vaultKeysMappings.CreateVaultKeysDataModel(vaultKeyEncrypted, returningVault);

            await _vaultKeysRepo.AddAsync(vaultKeys);

            addVaultKeyResponse.Success = true;
            addVaultKeyResponse.TimeOfKeyCreation = vaultKeys.DateTimeVaultKeyCreated;
            addVaultKeyResponse.VaultName = returningVault.VaultName;
            addVaultKeyResponse.HashedKey = vaultKeys.HashedVaultKey;
            addVaultKeyResponse.KeyName = addVaultKey.KeyName;

            return addVaultKeyResponse;

        }

        public async Task<RemoveVaultKeyReturn> RemoveVaultKey(RemoveVaultKeyDto removeVaultKey, string shortLivedToken)
        {
            RemoveVaultKeyReturn removeVaultKeyResponse = new RemoveVaultKeyReturn();

            string? accountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(accountIdFromToken == string.Empty)
            {
                removeVaultKeyResponse.Success = false;
                removeVaultKeyResponse.VaultId = "";
                removeVaultKeyResponse.KeyName = "";

                return removeVaultKeyResponse;
            }

            VaultKeysDataModel removingVaultKey = await _vaultKeysRepo.RemoveVaultKeyViaKeyId(Guid.Parse(removeVaultKey.VaultKeyId));

            if (removingVaultKey == null)
            {
                removeVaultKeyResponse.Success = false;
                removeVaultKeyResponse.VaultId = "";
                removeVaultKeyResponse.KeyName = "";

                return removeVaultKeyResponse;
            }

            removeVaultKeyResponse.Success = true;
            removeVaultKeyResponse.VaultId = removingVaultKey.VaultId.ToString();
            removeVaultKeyResponse.KeyName = removingVaultKey.KeyName;

            return removeVaultKeyResponse;


        }

        public async Task<UpdateVaultKeyReturn> UpdateVaultKey()
        {
            throw new NotImplementedException();
        }

        public async Task<DecryptVaultKeyReturn> DecryptVaultKey(DecryptVaultKeyDto decryptVaultkey, string shortLivedToken)
        {
            DecryptVaultKeyReturn decryptVaultKeyResponse = new DecryptVaultKeyReturn();

            string? getAccountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(getAccountIdFromToken == null)
            {
                decryptVaultKeyResponse.Sucess = false;
                decryptVaultKeyResponse.DecryptedVaultKey = "";

                return decryptVaultKeyResponse;

            }

            bool userHasAccountAndVaults = await _vaultRepo.HasVault(Guid.Parse(getAccountIdFromToken), Guid.Parse(decryptVaultkey.VaultId));

            if (!userHasAccountAndVaults)
            {
                decryptVaultKeyResponse.Sucess = false;
                decryptVaultKeyResponse.DecryptedVaultKey = "";

                return decryptVaultKeyResponse;
            }

            string decryptedVaultKey = DecryptKey(decryptVaultkey.EncryptedVaultKey);

            decryptVaultKeyResponse.DecryptedVaultKey = decryptedVaultKey;

            return decryptVaultKeyResponse;
        }

        public async Task ReturnAllVaultsForUser()
        {
            throw new NotImplementedException();
        }

        private string EncryptVaultKey(string keyToEncrypt)
        {

            byte[] encryptedKey = _aes.EncryptFromString(keyToEncrypt);

            return Convert.ToBase64String(encryptedKey);


        }

        private string DecryptKey(string encryptedVaultKey)
        {
            byte[] encryptedKey = Convert.FromBase64String(encryptedVaultKey); 

            return _aes.DecryptToString(encryptedKey);
        }
    }
}
