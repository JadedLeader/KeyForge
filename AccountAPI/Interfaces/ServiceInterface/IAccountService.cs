using gRPCIntercommunicationService;
using KeyForgedShared.DTO_s.AccountDTO_s;
using KeyForgedShared.ReturnTypes.Accounts;

namespace AccountAPI.Interfaces.ServiceInterface
{
    public interface IAccountService
    {

        public Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request);

        public Task<DeleteAccountResponse> RemoveAccount(DeleteAccountRequest request);

        public Task<GetAccountDetailsReturn> GetAccountDetails(string shortLivedToken);

        public Task<CheckPasswordMatchReturn> CheckPasswordMatch(PasswordMatchDto passwordMatchRequest, string shortLivedToken);

    }
}
