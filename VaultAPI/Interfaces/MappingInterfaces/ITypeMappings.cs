using gRPCIntercommunicationService.Protos;
using KeyForgedShared.SharedDataModels;

namespace VaultAPI.Interfaces.MappingInterfaces
{
    public interface ITypeMappings
    {
        public VaultDataModel CreateVaultDataModel(Guid accountId, string vaultName, AccountDataModel account);
        StreamVaultCreationsResponse MapVaultModelToStreamVault(VaultDataModel vaultModel);
        StreamVaultDeletionsResponse MapVaultToStreamVaultDeletions(VaultDataModel vaultModel);
        StreamVaultUpdateResponse MapVaultToStreamVaultUpdates(VaultDataModel vaultModel);
    }
}