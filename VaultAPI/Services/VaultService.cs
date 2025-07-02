using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.Interfaces;
using Serilog;
using VaultAPI.DataModel;
using VaultAPI.Interfaces.RepoInterfaces;
using VaultAPI.Interfaces.ServiceInterfaces;
using VaultAPI.Repos;

namespace VaultAPI.Services
{
    public class VaultService : Vault.VaultBase, IVaultService
    {

        private readonly IJwtHelper _jwtHelper;

        private readonly IAccountRepo _accountRepo;

        private readonly IVaultRepo _vaultRepo;
        public VaultService(IJwtHelper jwtHelper, IAccountRepo accountRepo, IVaultRepo vaultRepo)
        {
            _jwtHelper = jwtHelper;
            _accountRepo = accountRepo;
            _vaultRepo = vaultRepo;
        }

        public async Task<CreateVaultResponse> CreateVault(CreateVaultRequest request, string shortLivedToken)
        {

            CreateVaultResponse serverResponse = new CreateVaultResponse();

            string? accountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            Guid parsedAccountId = Guid.Parse(accountIdFromToken);

            if (accountIdFromToken == string.Empty)
            {

                Log.Warning($"No account ID can be found within the JWT");

                serverResponse.AccountId = string.Empty;
                serverResponse.VaultId = string.Empty;
                serverResponse.Sucessfull = false;
                serverResponse.VaultName = string.Empty;

                return serverResponse;
            }

            AccountDataModel? doesAccountExist = await _accountRepo.CheckForExistingAccount(parsedAccountId);

            if (doesAccountExist.AccountId == Guid.Empty)
            {
                Log.Warning($"No account can be identified within the database based off the provided ID");

                serverResponse.AccountId = accountIdFromToken;
                serverResponse.VaultId= string.Empty;
                serverResponse.VaultName = string.Empty;
                serverResponse.Sucessfull = false;

                return serverResponse;
            }

            var vaultType = (VaultAPI.DataModel.VaultType) Enum.Parse(typeof(VaultAPI.DataModel.VaultType), request.VaultType);

            VaultDataModel createNewVault = CreateVaultDataModel(Guid.Parse(accountIdFromToken), request.VaultName, vaultType, doesAccountExist);

            await _vaultRepo.AddAsync(createNewVault);

            serverResponse.AccountId = createNewVault.AccountId.ToString();
            serverResponse.VaultName = createNewVault.VaultName;
            serverResponse.VaultType = (gRPCIntercommunicationService.Protos.VaultType)createNewVault.VaultType;
            serverResponse.VaultId = createNewVault.VaultId.ToString();
            serverResponse.Sucessfull = true;

            return serverResponse;


        }

        public async Task<DeleteVaultResponse> DeleteVault(DeleteVaultRequest request, string shortLivedToken, string vaultIdCookie)
        {
            DeleteVaultResponse serverResponse = new DeleteVaultResponse();

            string? getShortLivedToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(getShortLivedToken == string.Empty)
            {

                Log.Warning($"no short lived token provided when attempting to delete a vault");

                serverResponse.AccountId = string.Empty; 
                serverResponse.VaultId = string.Empty;
                serverResponse.Successfull = false; 

                return serverResponse;
            }

            AccountDataModel doesAccountExist = await _accountRepo.CheckForExistingAccount(Guid.Parse(getShortLivedToken));

            VaultDataModel checkForExistingVault = await _vaultRepo.GetVaultViaVaultId(Guid.Parse(vaultIdCookie));

            if (doesAccountExist.AccountId == Guid.Empty || checkForExistingVault.VaultId == Guid.Empty)
            {
                Log.Warning($"account doesn't exist in the database");

                serverResponse.AccountId = getShortLivedToken;
                serverResponse.VaultId = string.Empty;
                serverResponse.Successfull = false;

                return serverResponse;
            }

            await _vaultRepo.DeleteAsync(checkForExistingVault);

            serverResponse.VaultId = checkForExistingVault.VaultId.ToString();
            serverResponse.AccountId = checkForExistingVault.AccountId.ToString();
            serverResponse.Successfull = true;

            return serverResponse;

        }

        public async Task<UpdateVaultNameResponse> UpdateVaultName(UpdateVaultNameRequest request, string shortLivedToken, string vaultIdCookie)
        {
            UpdateVaultNameResponse serverResponse = new UpdateVaultNameResponse();

            string? getAccountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(getAccountIdFromToken == string.Empty)
            {
                Log.Warning($"No account ID could be retrieved from the short lived token");

                serverResponse.VaultId = string.Empty; 
                serverResponse.UpdatedVaultName = string.Empty;
                serverResponse.Successfull = false; 

                return serverResponse;
            }

            VaultDataModel existingVault = await _vaultRepo.GetVaultViaVaultId(Guid.Parse(vaultIdCookie));

            if(existingVault.VaultId == Guid.Empty)
            {
                Log.Warning($"No vault found, cannot update vault name without a designated vault for account {getAccountIdFromToken}");

                serverResponse.VaultId = string.Empty;
                serverResponse.UpdatedVaultName= string.Empty;
                serverResponse.Successfull = false;

                return serverResponse;
            }

            await _vaultRepo.UpdateVaultName(existingVault, request.VaultName);

            serverResponse.VaultId = existingVault.VaultId.ToString();
            serverResponse.UpdatedVaultName = existingVault.VaultName;
            serverResponse.Successfull = true;

            return serverResponse;



        }

        private VaultDataModel CreateVaultDataModel(Guid accountId, string vaultName, VaultAPI.DataModel.VaultType vaultType, AccountDataModel account)
        {
            VaultDataModel newVault = new VaultDataModel
            {
                VaultId = Guid.NewGuid(),
                AccountId = accountId,
                VaultName = vaultName,
                VaultType = vaultType,
                VaultCreatedAt = DateTime.Now,
                Account = account,
            };

            return newVault;


        }





    }
}
