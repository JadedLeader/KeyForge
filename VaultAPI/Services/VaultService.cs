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
using System.Collections.Immutable;

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

            bool vaultNameEmpty = string.IsNullOrWhiteSpace(request.VaultName);

            if(vaultNameEmpty)
            {
                vaultReturn.Sucessful = false; 
                return vaultReturn;
            }

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

            if (doesAccountExist.Id == Guid.Empty)
            {
                Log.Warning($"No account can be identified within the database based off the provided ID");

                vaultReturn.AccountId = accountIdFromToken;
                vaultReturn.VaultId= string.Empty;
                vaultReturn.VaultName = string.Empty;
                vaultReturn.Sucessful = false;

                return vaultReturn;
            }

            VaultDataModel createNewVault = _typeMappings.CreateVaultDataModel(Guid.Parse(accountIdFromToken), request.VaultName, doesAccountExist);

            await _vaultRepo.AddAsync(createNewVault);

            vaultReturn.AccountId = createNewVault.AccountId.ToString();
            vaultReturn.VaultName = createNewVault.VaultName;
            vaultReturn.VaultType = (KeyForgedShared.ReturnTypes.Vaults.VaultType)createNewVault.VaultType;
            vaultReturn.VaultId = createNewVault.Id.ToString();
            vaultReturn.Sucessful = true;

            StreamVaultCreationsResponse newVaultCreationResponse = _typeMappings.MapVaultModelToStreamVault(createNewVault);

            _vaultActionsStorage.AddToVaultCreations(newVaultCreationResponse);

            return vaultReturn;


        }

        public async Task<DeleteVaultReturn> DeleteVault(DeleteVaultDto request, string shortLivedToken)
        {
            DeleteVaultReturn serverResponse = new DeleteVaultReturn();

            bool empty = string.IsNullOrWhiteSpace(request.VaultId);

            if (empty)
            {
                serverResponse.Sucessful = false; 

                return serverResponse;
            }

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

            VaultDataModel checkForExistingVault = await _vaultRepo.GetVaultViaVaultId(Guid.Parse(request.VaultId));

            if (doesAccountExist.Id == Guid.Empty || checkForExistingVault.Id == Guid.Empty)
            {
                Log.Warning($"account doesn't exist in the database");

                serverResponse.AccountId = getShortLivedToken;
                serverResponse.VaultId = string.Empty;
                serverResponse.Sucessful = false;

                return serverResponse;
            }

            await _vaultRepo.DeleteAsync(checkForExistingVault);

            serverResponse.VaultId = checkForExistingVault.Id.ToString();
            serverResponse.AccountId = checkForExistingVault.AccountId.ToString();
            serverResponse.Sucessful = true;

            StreamVaultDeletionsResponse newDeleteVaultResponse = _typeMappings.MapVaultToStreamVaultDeletions(checkForExistingVault);

            _vaultActionsStorage.AddToVaultDeletions(newDeleteVaultResponse);

            return serverResponse;

        }

        public async Task<UpdateVaultNameReturn> UpdateVaultName(UpdateVaultNameDto request, string shortLivedToken)
        {
            UpdateVaultNameReturn serverResponse = new UpdateVaultNameReturn();

            bool vaultNameChange = string.IsNullOrWhiteSpace(request.VaultName);

            if(vaultNameChange)
            {
                serverResponse.Sucessful = true;
                serverResponse.Description = "No change occurred";

                return serverResponse;
            }

            string? getAccountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(getAccountIdFromToken == string.Empty)
            {
                Log.Warning($"No account ID could be retrieved from the short lived token");

                serverResponse.VaultId = string.Empty; 
                serverResponse.UpdatedVaultName = string.Empty;
                serverResponse.Sucessful = false; 

                return serverResponse;
            }

            VaultDataModel existingVault = await _vaultRepo.GetVaultViaVaultId(Guid.Parse(request.VaultId));

            if(existingVault.Id == Guid.Empty)
            {
                Log.Warning($"No vault found, cannot update vault name without a designated vault for account {getAccountIdFromToken}");

                serverResponse.VaultId = string.Empty;
                serverResponse.UpdatedVaultName= string.Empty;
                serverResponse.Sucessful = false;

                return serverResponse;
            }

            await _vaultRepo.UpdateVaultName(existingVault, request.VaultName);

            serverResponse.VaultId = existingVault.Id.ToString();
            serverResponse.UpdatedVaultName = request.VaultName;
            serverResponse.Sucessful = true;

            StreamVaultUpdateResponse newStreamVaultUpdate =  _typeMappings.MapVaultToStreamVaultUpdates(existingVault);
            
            _vaultActionsStorage.AddToVaultUpdates(newStreamVaultUpdate);
            
            return serverResponse;

        }

        public async Task<bool> DeleteAllVaults(string shortLivedToken)
        {
            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if(accountId == Guid.Empty)
            {
                return false;
            }

            await _vaultRepo.DeleteAllVaultsForAccount(accountId);

            return true;
        }

        public override async Task StreamVaultCreations(StreamVaultCreationsRequest request, IServerStreamWriter<StreamVaultCreationsResponse> responseStream, ServerCallContext context)
        {
            Log.Information($"StreamVaultCreations client connected");

            while (!context.CancellationToken.IsCancellationRequested)
            {

                var creations = _vaultActionsStorage.ReturnVaultCreations().ToImmutableList();

                if(creations.Count == 0)
                {
                    continue;
                }

                foreach (var vaultCreation in creations)
                {
                    Log.Information($"vault creation being sent {vaultCreation.VaultId} : {vaultCreation.VaultName}");

                    await responseStream.WriteAsync(vaultCreation);
                }

                _vaultActionsStorage.ClearVaultCreationsDict();

                await Task.Delay(250);
            }

            
        }

        public override async Task StreamVaultDeletions(StreamVaultDeletionsRequest request, IServerStreamWriter<StreamVaultDeletionsResponse> responseStream, ServerCallContext context)
        {
            Log.Information("StreamVaultDeletions CLIENT CONNECTED.");

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var deletions = _vaultActionsStorage.ReturnVaultDeletions().ToImmutableList();

                if(deletions.Count == 0)
                {
                    continue;
                }

                foreach (var deletion in deletions)
                {
                    Log.Information($"Sending vault deletion: {deletion.VaultId}");
                    await responseStream.WriteAsync(deletion);
                }

                
                _vaultActionsStorage.ClearVaultDeletionsDict();

                await Task.Delay(250); 
            }
        }

        public override async Task StreamVaultUpdates(StreamVaultUpdateRequest request, IServerStreamWriter<StreamVaultUpdateResponse> responseStream, ServerCallContext context)
        {

            Log.Information($"StreamVaultUpdates client connected");

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var updates = _vaultActionsStorage.ReturnVaultUpdates().ToImmutableList();

                if(updates.Count == 0)
                {
                    continue;
                }

                foreach (var vaultUpdate in updates)
                {
                    Log.Information($"Sending vault update: {vaultUpdate.VaultId} : {vaultUpdate.VaultName}");
                    await responseStream.WriteAsync(vaultUpdate);
                }

                _vaultActionsStorage.ClearVaultUpdatesDict();

                await Task.Delay(250);

            }



          
        }
     
    }
}
