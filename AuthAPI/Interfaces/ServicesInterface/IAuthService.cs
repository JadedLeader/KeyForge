using gRPCIntercommunicationService.Protos;

namespace AuthAPI.Interfaces.ServicesInterface
{
    public interface IAuthService
    {
        public Task<CreateAuthAccountResponse> CreateAuthAccount(CreateAuthAccountRequest request);

        public Task<RefreshLongLivedTokenResponse> RefreshLongLivedToken(RefreshLongLivedTokenRequest request);

    }
}
