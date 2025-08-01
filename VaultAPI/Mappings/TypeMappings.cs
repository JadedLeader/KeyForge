using gRPCIntercommunicationService.Protos;
using VaultAPI.DataModel;
using VaultAPI.Interfaces.MappingInterfaces;

namespace VaultAPI.Mappings
{
    public class TypeMappings : ITypeMappings
    {

        public StreamVaultCreationsResponse MapVaultModelToStreamVault(VaultDataModel vaultModel)
        {
            StreamVaultCreationsResponse newVaultCreationResponse = new StreamVaultCreationsResponse
            {
                VaultId = vaultModel.VaultId.ToString(),
                AccountId = vaultModel.AccountId.ToString(),
                VaultName = vaultModel.VaultName,
                VaultCreatedAt = vaultModel.VaultCreatedAt.ToString(),
                VaultType = (gRPCIntercommunicationService.Protos.VaultType)vaultModel.VaultType,
            };

            return newVaultCreationResponse;

        }

        public VaultDataModel CreateVaultDataModel(Guid accountId, string vaultName, VaultAPI.DataModel.VaultType vaultType, AccountDataModel account)
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

        public StreamVaultDeletionsResponse MapVaultToStreamVaultDeletions(VaultDataModel vaultModel)
        {
            StreamVaultDeletionsResponse newVaultDeletionResponse = new StreamVaultDeletionsResponse
            {
                VaultId = vaultModel.VaultId.ToString()
            }; 

            return newVaultDeletionResponse;
        }

        public StreamVaultUpdateResponse MapVaultToStreamVaultUpdates(VaultDataModel vaultModel)
        {
            StreamVaultUpdateResponse streamVaultUpdates = new StreamVaultUpdateResponse
            {
                UniqueVaultUpdateId = Guid.NewGuid().ToString(),
                AccountId = vaultModel.AccountId.ToString(),
                VaultName = vaultModel.VaultName,
                VaultId = vaultModel.VaultId.ToString(),

            };


            return streamVaultUpdates;
        }

        

    }
}
