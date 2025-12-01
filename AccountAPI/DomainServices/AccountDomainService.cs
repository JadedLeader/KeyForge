using AccountAPI.Interfaces.DomainServices;
using AccountAPI.Interfaces.RepoInterface;
using KeyForgedShared.DTO_s.AccountDTO_s;
using KeyForgedShared.SharedDataModels;
using System.Threading.Tasks;

namespace AccountAPI.DomainServices
{
    public class AccountDomainService : IAccountDomainService
    {

        private readonly IAccountRepo _accountRepo;

        public AccountDomainService(IAccountRepo accountRepo)
        {
            _accountRepo = accountRepo;
        }

        private bool ValidateCreateAccountInput(CreateAccountDto createAccount)
        {
            if (string.IsNullOrWhiteSpace(createAccount.Username) || string.IsNullOrWhiteSpace(createAccount.Password) || string.IsNullOrWhiteSpace(createAccount.Email))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateEmailDoesntAlreadyExist(string email)
        {
            if (await _accountRepo.EmailAlreadyExists(email))
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ValidateCreateAccount(CreateAccountDto createAccount)
        {
            if (!ValidateCreateAccountInput(createAccount))
            {
                return false;
            }

            if (await ValidateEmailDoesntAlreadyExist(createAccount.Email))
            {
                return false;
            }

            return true;
        }

        private bool ValidateDeleteAccountInput(DeleteAccountDto deleteAccount)
        {
            if (string.IsNullOrWhiteSpace(deleteAccount.AccountId))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateExistingAccount(Guid accountId)
        {
            AccountDataModel? hasAccount = await _accountRepo.FindSingleRecordViaId<AccountDataModel>(accountId);

            if (hasAccount == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateDeleteAccount(DeleteAccountDto deleteAccount)
        {
            if (!ValidateDeleteAccountInput(deleteAccount))
            {
                return false;
            }

            if (!await ValidateExistingAccount(Guid.Parse(deleteAccount.AccountId)))
            {

                return false;
            }

            return true;
        }

        public async Task<AccountDataModel> DeleteAccount(Guid accountId)
        {
            AccountDataModel? accountToDelete = await _accountRepo.DeleteRecordViaId<AccountDataModel>(accountId);

            if (accountToDelete == null)
            {
                return null;
            }

            return accountToDelete;
        }



    }
}
