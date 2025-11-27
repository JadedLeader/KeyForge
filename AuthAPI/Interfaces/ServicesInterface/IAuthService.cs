using gRPCIntercommunicationService.Protos;
using KeyForgedShared.DTO_s.AuthDTO_s;
using KeyForgedShared.ReturnTypes.Auth;

namespace AuthAPI.Interfaces.ServicesInterface
{
    public interface IAuthService
    {
        public Task<CreateAuthReturn> CreateAuthAccount(CreateAuthDto request);

        public Task<RefreshLongLivedTokenReturn> RefreshLongLivedToken(RefreshLongLivedTokenDto request);

        public Task<RefreshShortLivedTokenReturn> RefreshShortLivedToken(RefreshShortLivedTokenDto request);

        public Task<RevokeLongLivedTokenReturn> RevokeLongLivedToken(RevokeLongLivedTokenDto request);

        public Task<LoginReturn> Login(LoginDto request);

        public Task<ReinstateAuthKeyReturn> ReinstantiateAuthKey(ReinstateAuthKeyDto request);

        public Task<SilentShortLivedTokenRefreshReturn> SilentTokenCycle(string longLivedToken);

    }
}
