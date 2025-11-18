using KeyForgedShared.DTO_s.VaultKeysDTO_s;
using KeyForgedShared.Interfaces;
using KeyForgedShared.Projections.VaultKeysProjections;
using KeyForgedShared.ReturnTypes.VaultKeys;
using KeyForgedShared.SharedDataModels;
using Microsoft.AspNetCore.Components.Web;
using NETCore.Encrypt;
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

        private readonly string _key;
        
        public VaultKeysService(IVaultKeysRepo vaultKeysRepo, IJwtHelper jwtHelper, IVaultRepo vaultRepo, IConfiguration config, VaultKeysMappings vaultKeyMappings)
        {
            _vaultKeysRepo = vaultKeysRepo;
            _jwtHelper = jwtHelper;
            _vaultRepo = vaultRepo;
            _configuration = config;
            _vaultKeysMappings = vaultKeyMappings;

            _key = _configuration["Vault:AesKey"];
                

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

            VaultDataModel? getVault = await GetVaultWithRetry(Guid.Parse(addVaultKey.VaultId));

            if(getVault == null)
            {
                addVaultKeyResponse.Success = false;
                addVaultKeyResponse.TimeOfKeyCreation = string.Empty;
                addVaultKeyResponse.VaultName = string.Empty;
                addVaultKeyResponse.HashedKey = string.Empty;
                addVaultKeyResponse.KeyName = string.Empty;

                return addVaultKeyResponse;
            }

            string vaultKeyEncrypted = EncryptVaultKey(addVaultKey.PasswordToEncrypt);

            VaultKeysDataModel vaultKeys = _vaultKeysMappings.CreateVaultKeysDataModel(vaultKeyEncrypted, getVault, addVaultKey.KeyName);

            await _vaultKeysRepo.AddAsync(vaultKeys);

            addVaultKeyResponse.Success = true;
            addVaultKeyResponse.TimeOfKeyCreation = vaultKeys.DateTimeVaultKeyCreated;
            addVaultKeyResponse.VaultName = getVault.VaultName;
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

            VaultKeysDataModel removingVaultKey = await _vaultKeysRepo.RemoveVaultKeyViaKeyId(Guid.Parse(removeVaultKey.VaultKeyId), Guid.Parse(removeVaultKey.VaultId));

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

            await _vaultKeysRepo.DeleteAsync(removingVaultKey);

            return removeVaultKeyResponse;


        }

        public async Task<UpdateVaultKeyReturn> UpdateVaultKey(UpdateVaultKeyDto updateVaultKey, string shortLivedToken)
        {

            UpdateVaultKeyReturn updateVaultKeyResponse = new UpdateVaultKeyReturn();

            string? getAccountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(getAccountIdFromToken == null)
            {
                updateVaultKeyResponse.VaultName = "";
                updateVaultKeyResponse.VaultKeyName = "";
                updateVaultKeyResponse.EncryptedVaultKey = "";

                return updateVaultKeyResponse;
            }

            SingleVaultWithSingleKeyProjection vaultKeysModel = await _vaultKeysRepo.ReturnVaultAndKey(Guid.Parse(updateVaultKey.VaultId), Guid.Parse(getAccountIdFromToken));

            if (!vaultEncryptedKeyChanged(updateVaultKey) && vaultKeyNameChanged(updateVaultKey))
            {
                updateVaultKeyResponse.VaultName = vaultKeysModel.VaultName;
                updateVaultKeyResponse.EncryptedVaultKey = vaultKeysModel.singleVaultKey.EncryptedVaultKey;
                updateVaultKeyResponse.VaultKeyName = vaultKeysModel.singleVaultKey.KeyName;

                return updateVaultKeyResponse;
            }
            else if(!vaultEncryptedKeyChanged(updateVaultKey))
            {

            }
            else if(!vaultKeyNameChanged(updateVaultKey))
            {

            }

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
            decryptVaultKeyResponse.Sucess = true;

            return decryptVaultKeyResponse;
        }

        public async Task<List<GetAllVaultsDto>> ReturnAllVaultsForUser(string shortLivedToken)
        {

            string? accountId = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(accountId == null)
            {
                return null;
            }

            List<GetAllVaultsDto> getAllVaults = await _vaultKeysRepo.ReturnVaultKeys(Guid.Parse(accountId));

            if(getAllVaults.Count == 0)
            {
                new List<GetAllVaultsDto>();
            }

            return getAllVaults;



        }

        public async Task<CascadeDeleteVaultKeysReturn> CascadeVaultKeyDeleteFromVault(CascadeVaultKeyDeleteDto cascadeVaultDeleteRequest,string shortLivedToken)
        {

            CascadeDeleteVaultKeysReturn cascadeDeleteResponse = new CascadeDeleteVaultKeysReturn();

            string? accountId = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(accountId == null)
            {
                cascadeDeleteResponse.VaultId = "";
                cascadeDeleteResponse.Success = false;

                return cascadeDeleteResponse;
            }

            bool hasVault = await _vaultRepo.HasVault(Guid.Parse(accountId), Guid.Parse(cascadeVaultDeleteRequest.VaultId));

            if (!hasVault)
            {
                cascadeDeleteResponse.VaultId = "";
                cascadeDeleteResponse.Success = false;

                return cascadeDeleteResponse;
            }

            VaultDataModel cascadeDelete = await _vaultRepo.CascadeDeleteIntoVaultKeys(Guid.Parse(cascadeVaultDeleteRequest.VaultId));

            cascadeDeleteResponse.VaultId = cascadeDelete.VaultId.ToString();
            cascadeDeleteResponse.Success = true; 

            return cascadeDeleteResponse;

    
        }
        public async Task<RemoveAllVaultKeysReturn> RemoveAllVaultKeys(RemoveAllVaultKeysDto removeAllVaultKeys, string shortLivedToken)
        {
            RemoveAllVaultKeysReturn removeAllVaultKeysReturn = new RemoveAllVaultKeysReturn();

            string? accountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if (accountIdFromToken == string.Empty)
            {
                removeAllVaultKeysReturn.VaultId = "";
                removeAllVaultKeysReturn.KeysDeleted = new List<Guid>();
                removeAllVaultKeysReturn.Success = false;

                return removeAllVaultKeysReturn;
            }

            bool ensuringUserHasVault = await _vaultRepo.HasVault(Guid.Parse(accountIdFromToken), Guid.Parse(removeAllVaultKeys.VaultId));

            if(!ensuringUserHasVault)
            {
                removeAllVaultKeysReturn.VaultId = "";
                removeAllVaultKeysReturn.KeysDeleted = new List<Guid>();
                removeAllVaultKeysReturn.Success = false;

                return removeAllVaultKeysReturn;
            }

            List<Guid> guidsRemoved = await _vaultKeysRepo.RemoveAllVaultsKeysFromVault(Guid.Parse(removeAllVaultKeys.VaultId));

            if(guidsRemoved == null)
            {
                removeAllVaultKeysReturn.VaultId = "";
                removeAllVaultKeysReturn.KeysDeleted = new List<Guid>();
                removeAllVaultKeysReturn.Success = false;

                return removeAllVaultKeysReturn;
            }


            removeAllVaultKeysReturn.VaultId = removeAllVaultKeys.VaultId;
            
            foreach(var keys in guidsRemoved)
            {
                removeAllVaultKeysReturn.KeysDeleted.Add(keys);
            }

            removeAllVaultKeysReturn.Success = true;

            return removeAllVaultKeysReturn;
            

        }

        public async Task<GetSingleVaultWithAllDetailsReturn> GetSingleVaultWithAllKeysAndDetails(GetSingleVaultWithAllDetailsDto getSingleVaultWithAllDetails, string shortLivedToken)
        {
            GetSingleVaultWithAllDetailsReturn getSingleVaultResponse = new GetSingleVaultWithAllDetailsReturn();

            string? accountId = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(accountId == null)
            {
                getSingleVaultResponse.Success = false;

                return getSingleVaultResponse;
            }

            bool hasVault = await _vaultRepo.HasVault(Guid.Parse(accountId), Guid.Parse(getSingleVaultWithAllDetails.VaultId));

            bool hasVaultKeys = await _vaultKeysRepo.HasVaultKeys(Guid.Parse(getSingleVaultWithAllDetails.VaultId)); 

            if(!hasVault || !hasVaultKeys)
            {
                getSingleVaultResponse.Success = false;

                return getSingleVaultResponse;
            }

            GetSingleVaultWithAllKeysAndDetailsProjection? getVaultWithAllDetails = await _vaultKeysRepo.GetAllDetailsForVault(Guid.Parse(getSingleVaultWithAllDetails.VaultId));

            if(getVaultWithAllDetails == null)
            {
                getSingleVaultResponse.Success = false;

                return getSingleVaultResponse;
            }

            GetSingleVaultWithAllDetailsReturn response = _vaultKeysMappings.MapProjectionToGetSingleVaultAllDetails(getVaultWithAllDetails);

            return response;


        }


        private string EncryptVaultKey(string keyToEncrypt)
        {
           
            return EncryptProvider.AESEncrypt(keyToEncrypt, _key);
        }

        private string DecryptKey(string encryptedVaultKey)
        {
            return EncryptProvider.AESDecrypt(encryptedVaultKey, _key);
        }

        private bool vaultKeyNameChanged(UpdateVaultKeyDto updateVaultKey)
        {
            if(updateVaultKey.ChangedKeyName == string.Empty)
            {
                return false;
            }

            return true;
        }

        private bool vaultEncryptedKeyChanged(UpdateVaultKeyDto updateVaultKey)
        {
            if(updateVaultKey.ChangedEncryptedVaultKey == string.Empty)
            {
                return false;
            }

            return true;
        }

        private async Task<VaultDataModel?> GetVaultWithRetry(Guid vaultId, int maxRetries = 5, int delayMs = 100)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                var vault = await _vaultRepo.GetVaultByVaultId(vaultId);
                if (vault != null)
                    return vault;

                await Task.Delay(delayMs);
            }

            return null;
        }

        


    }
}
