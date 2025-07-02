using gRPCIntercommunicationService.Protos;

namespace VaultAPI.Interfaces.ServiceInterfaces
{
    public interface IVaultService
    {
        Task<CreateVaultResponse> CreateVault(CreateVaultRequest request, string shortLivedToken);

        Task<DeleteVaultResponse> DeleteVault(DeleteVaultRequest request, string shortLivedToken, string vaultIdCookie);

        Task<UpdateVaultNameResponse> UpdateVaultName(UpdateVaultNameRequest request, string shortLivedToken, string vaultIdCookie);
    }
}