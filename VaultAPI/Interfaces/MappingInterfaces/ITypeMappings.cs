using gRPCIntercommunicationService.Protos;
using KeyForgedShared.SharedDataModels;

namespace VaultAPI.Interfaces.MappingInterfaces
{
    public interface ITypeMappings
    {
        VaultDataModel CreateVaultDataModel(Guid accountId, string vaultName, KeyForgedShared.SharedDataModels.VaultType vaultType, AccountDataModel account);
        StreamVaultCreationsResponse MapVaultModelToStreamVault(VaultDataModel vaultModel);
        StreamVaultDeletionsResponse MapVaultToStreamVaultDeletions(VaultDataModel vaultModel);
        StreamVaultUpdateResponse MapVaultToStreamVaultUpdates(VaultDataModel vaultModel);
    }
}