using gRPCIntercommunicationService.Protos;
using KeyForgedShared.DTO_s.VaultDTO_s;
using KeyForgedShared.ReturnTypes.Vaults;

namespace VaultAPI.Interfaces.ServiceInterfaces
{
    public interface IVaultService
    {
        Task<CreateVaultReturn> CreateVault(CreateVaultDto request, string shortLivedToken);

        Task<DeleteVaultReturn> DeleteVault(DeleteVaultDto request, string shortLivedToken);

        Task<UpdateVaultNameReturn> UpdateVaultName(UpdateVaultNameDto request, string shortLivedToken, string vaultIdCookie);
    }
}