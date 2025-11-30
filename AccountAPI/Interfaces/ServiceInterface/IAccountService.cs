using gRPCIntercommunicationService;
using KeyForgedShared.DTO_s.AccountDTO_s;
using KeyForgedShared.ReturnTypes.Accounts;

namespace AccountAPI.Interfaces.ServiceInterface
{
    public interface IAccountService
    {

     

        public Task<CreateAccountReturn> CreateAccount(CreateAccountDto request);

        public Task<DeleteAccountReturn> RemoveAccount(DeleteAccountDto request);

        public Task<GetAccountDetailsReturn> GetAccountDetails(string shortLivedToken);

        public Task<CheckPasswordMatchReturn> CheckPasswordMatch(PasswordMatchDto passwordMatchRequest, string shortLivedToken);

    }
}
