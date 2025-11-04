using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.Interfaces;
using Serilog;
using KeyForgedShared.SharedDataModels;
using VaultAPI.Interfaces.MappingInterfaces;
using VaultAPI.Interfaces.RepoInterfaces;
using VaultAPI.Interfaces.ServiceInterfaces;
using VaultAPI.Repos;
using VaultAPI.Storage;
using KeyForgedShared.DTO_s.VaultDTO_s;
using KeyForgedShared.ReturnTypes.Vaults;

namespace VaultAPI.Services
{
    public class VaultService : Vault.VaultBase, IVaultService
    {

        private readonly IJwtHelper _jwtHelper;

        private readonly IAccountRepo _accountRepo;

        private readonly IVaultRepo _vaultRepo;

        private readonly VaultActionsStorage _vaultActionsStorage;

        private readonly ITypeMappings _typeMappings;
        public VaultService(IJwtHelper jwtHelper, IAccountRepo accountRepo, IVaultRepo vaultRepo, VaultActionsStorage vaultActionsStorage, ITypeMappings typeMappings)
        {
            _jwtHelper = jwtHelper;
            _accountRepo = accountRepo;
            _vaultRepo = vaultRepo;
            _vaultActionsStorage = vaultActionsStorage;
            _typeMappings = typeMappings;
        }

        public async Task<CreateVaultReturn> CreateVault(CreateVaultDto request, string shortLivedToken)
        {

            CreateVaultReturn vaultReturn = new CreateVaultReturn();

            string? accountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            Guid parsedAccountId = Guid.Parse(accountIdFromToken);

            if (accountIdFromToken == string.Empty)
            {

                Log.Warning($"No account ID can be found within the JWT");

                vaultReturn.AccountId = string.Empty;
                vaultReturn.VaultId = string.Empty;
                vaultReturn.Sucessful = false;
                vaultReturn.VaultName = string.Empty;

                return vaultReturn;
            }

            AccountDataModel? doesAccountExist = await _accountRepo.CheckForExistingAccount(parsedAccountId);

            if (doesAccountExist.AccountId == Guid.Empty)
            {
                Log.Warning($"No account can be identified within the database based off the provided ID");

                vaultReturn.AccountId = accountIdFromToken;
                vaultReturn.VaultId= string.Empty;
                vaultReturn.VaultName = string.Empty;
                vaultReturn.Sucessful = false;

                return vaultReturn;
            }

            var vaultType = (KeyForgedShared.SharedDataModels.VaultType) Enum.Parse(typeof(KeyForgedShared.SharedDataModels.VaultType), request.VaultType);

            VaultDataModel createNewVault = _typeMappings.CreateVaultDataModel(Guid.Parse(accountIdFromToken), request.VaultName, vaultType, doesAccountExist);

            await _vaultRepo.AddAsync(createNewVault);

            vaultReturn.AccountId = createNewVault.AccountId.ToString();
            vaultReturn.VaultName = createNewVault.VaultName;
            vaultReturn.VaultType = (KeyForgedShared.ReturnTypes.Vaults.VaultType)createNewVault.VaultType;
            vaultReturn.VaultId = createNewVault.VaultId.ToString();
            vaultReturn.Sucessful = true;

            StreamVaultCreationsResponse newVaultCreationResponse = _typeMappings.MapVaultModelToStreamVault(createNewVault);

            _vaultActionsStorage.AddToVaultCreations(newVaultCreationResponse);

            return vaultReturn;


        }

        public async Task<DeleteVaultReturn> DeleteVault(DeleteVaultDto request, string shortLivedToken, string vaultIdCookie)
        {
            DeleteVaultReturn serverResponse = new DeleteVaultReturn();

            string? getShortLivedToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(getShortLivedToken == string.Empty)
            {

                Log.Warning($"no short lived token provided when attempting to delete a vault");

                serverResponse.AccountId = string.Empty; 
                serverResponse.VaultId = string.Empty;
                serverResponse.Sucessful = false; 

                return serverResponse;
            }

            AccountDataModel doesAccountExist = await _accountRepo.CheckForExistingAccount(Guid.Parse(getShortLivedToken));

            VaultDataModel checkForExistingVault = await _vaultRepo.GetVaultViaVaultId(Guid.Parse(vaultIdCookie));

            if (doesAccountExist.AccountId == Guid.Empty || checkForExistingVault.VaultId == Guid.Empty)
            {
                Log.Warning($"account doesn't exist in the database");

                serverResponse.AccountId = getShortLivedToken;
                serverResponse.VaultId = string.Empty;
                serverResponse.Sucessful = false;

                return serverResponse;
            }

            await _vaultRepo.DeleteAsync(checkForExistingVault);

            serverResponse.VaultId = checkForExistingVault.VaultId.ToString();
            serverResponse.AccountId = checkForExistingVault.AccountId.ToString();
            serverResponse.Sucessful = true;

            StreamVaultDeletionsResponse newDeleteVaultResponse = _typeMappings.MapVaultToStreamVaultDeletions(checkForExistingVault);

            _vaultActionsStorage.AddToVaultDeletions(newDeleteVaultResponse);

            return serverResponse;

        }

        public async Task<UpdateVaultNameReturn> UpdateVaultName(UpdateVaultNameDto request, string shortLivedToken, string vaultIdCookie)
        {
            UpdateVaultNameReturn serverResponse = new UpdateVaultNameReturn();

            string? getAccountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(getAccountIdFromToken == string.Empty)
            {
                Log.Warning($"No account ID could be retrieved from the short lived token");

                serverResponse.VaultId = string.Empty; 
                serverResponse.UpdatedVaultName = string.Empty;
                serverResponse.Sucessful = false; 

                return serverResponse;
            }

            VaultDataModel existingVault = await _vaultRepo.GetVaultViaVaultId(Guid.Parse(vaultIdCookie));

            if(existingVault.VaultId == Guid.Empty)
            {
                Log.Warning($"No vault found, cannot update vault name without a designated vault for account {getAccountIdFromToken}");

                serverResponse.VaultId = string.Empty;
                serverResponse.UpdatedVaultName= string.Empty;
                serverResponse.Sucessful = false;

                return serverResponse;
            }

            await _vaultRepo.UpdateVaultName(existingVault, request.VaultName);

            serverResponse.VaultId = existingVault.VaultId.ToString();
            serverResponse.UpdatedVaultName = existingVault.VaultName;
            serverResponse.Sucessful = true;

            StreamVaultUpdateResponse newStreamVaultUpdate =  _typeMappings.MapVaultToStreamVaultUpdates(existingVault);
            
            _vaultActionsStorage.AddToVaultUpdates(newStreamVaultUpdate);
            

            return serverResponse;

        }

        public override async Task StreamVaultCreations(StreamVaultCreationsRequest request, IServerStreamWriter<StreamVaultCreationsResponse> responseStream, ServerCallContext context)
        {
            foreach(var vaultCreation in _vaultActionsStorage.ReturnVaultCreations())
            {
                await responseStream.WriteAsync(vaultCreation);
            }
        }

        public override async Task StreamVaultDeletions(StreamVaultDeletionsRequest request, IServerStreamWriter<StreamVaultDeletionsResponse> responseStream, ServerCallContext context)
        {
            foreach(var vaultDeletion in _vaultActionsStorage.ReturnVaultDeletions())
            {
                await responseStream.WriteAsync(vaultDeletion);
            }
        }

        public override async Task StreamVaultUpdates(StreamVaultUpdateRequest request, IServerStreamWriter<StreamVaultUpdateResponse> responseStream, ServerCallContext context)
        {
            foreach(var vaultUpdate in _vaultActionsStorage.ReturnVaultUpdates())
            {
                await responseStream.WriteAsync(vaultUpdate);
            }
        }
     
    }
}
