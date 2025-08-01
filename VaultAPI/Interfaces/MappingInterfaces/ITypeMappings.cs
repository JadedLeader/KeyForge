using gRPCIntercommunicationService.Protos;
using VaultAPI.DataModel;

namespace VaultAPI.Interfaces.MappingInterfaces
{
    public interface ITypeMappings
    {
        VaultDataModel CreateVaultDataModel(Guid accountId, string vaultName, DataModel.VaultType vaultType, AccountDataModel account);
        StreamVaultCreationsResponse MapVaultModelToStreamVault(VaultDataModel vaultModel);
        StreamVaultDeletionsResponse MapVaultToStreamVaultDeletions(VaultDataModel vaultModel);
        StreamVaultUpdateResponse MapVaultToStreamVaultUpdates(VaultDataModel vaultModel);
    }
}