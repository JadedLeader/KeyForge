using gRPCIntercommunicationService.Protos;

namespace AuthAPI.Interfaces.ServicesInterface
{
    public interface IAuthService
    {
        public Task<CreateAuthAccountResponse> CreateAuthAccount(CreateAuthAccountRequest request);

        public Task<RefreshLongLivedTokenResponse> RefreshLongLivedToken(RefreshLongLivedTokenRequest request);

        public Task<RefreshShortLivedTokenResponse> RefreshShortLivedToken(RefreshShortLivedTokenRequest request);

        public Task<RevokeLongLivedTokenResponse> RevokeLongLivedToken(RevokeLongLivedTokenRequest request);

        public Task<LoginResponse> Login(LoginRequest request);

        public Task<ReinstateAuthKeyResponse> ReinstantiateAuthKey(ReinstateAuthKeyRequest request);

        public Task<SilentShortLivedTokenRefreshResponse> SilentTokenCycle(SilentShortLivedTokenRefreshRequest request, string longLivedToken);

    }
}
