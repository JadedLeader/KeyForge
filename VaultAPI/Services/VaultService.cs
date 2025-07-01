using Grpc.Core;
using gRPCIntercommunicationService.Protos;

namespace VaultAPI.Services
{
    public class VaultService : Vault.VaultBase
    {

        public VaultService()
        {
            
        }

        public async Task<CreateVaultResponse> CreateVault(CreateVaultRequest request, string shortLivedToken)
        {
            
            throw new NotImplementedException();

        }



    }
}
