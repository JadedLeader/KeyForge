using gRPCIntercommunicationService;

namespace AccountAPI.Interfaces.ServiceInterface
{
    public interface IAccountService
    {

        public Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request);

        public Task<DeleteAccountResponse> RemoveAccount(DeleteAccountRequest request);

    }
}
