using AccountAPI.Interfaces.DomainServices;
using AccountAPI.Interfaces.RepoInterface;
using KeyForgedShared.DTO_s.AccountDTO_s;
using KeyForgedShared.ReturnTypes.Accounts;
using KeyForgedShared.SharedDataModels;
using KeyForgedShared.ValidationType;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
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

        private bool ValidateGetAccountDetailsInput(Guid accountId)
        {
            if(accountId == Guid.Empty)
            {
                return false; 
            }

            return true;
        }

        private async Task<AccountDataModel> ValidateUserHasAccount(Guid accountId)
        {
            AccountDataModel? accountModel = await _accountRepo.FindSingleRecordViaId<AccountDataModel>(accountId);

            if(accountModel == null)
            {
                return null; 
            }

            return accountModel;
        }

        public async Task<GetAccountValidationResult> ValidateGetAccountDetails(Guid accountId)
        {

            GetAccountValidationResult validationResult = new();

            if (!ValidateGetAccountDetailsInput(accountId))
            {
                validationResult.IsValidated = false; 

                return validationResult;
            }

            AccountDataModel account = await ValidateUserHasAccount(accountId);

            if(account == null)
            {
                validationResult.IsValidated = false; 

                return validationResult;
            }

            validationResult.IsValidated = true; 
            validationResult.AccountModel = account;

            return validationResult;
        }

        private bool ValidatePasswordMatchInput(PasswordMatchDto passwordMatch)
        {
            if (string.IsNullOrWhiteSpace(passwordMatch.Password))
            {
                return false;
            }

            return true;
        }

        private async Task<string> ValidateHashedPassword(Guid accountId)
        {
            string? getHashedPassword = await _accountRepo.GetHashedPassword(accountId); 

            if(getHashedPassword == null)
            {
                return null;
            }

            return getHashedPassword;
        }

        private bool ValidatePasswordMatchesHash(string passwordToCheck, string hashToCheck)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(passwordToCheck, hashToCheck);
        }

        public async Task<bool> ValidatePasswordMatch(PasswordMatchDto passwordMatch, Guid accountId)
        {

            if (!ValidatePasswordMatchInput(passwordMatch))
            {
                return false;
            }

            string? hashedPassword = await ValidateHashedPassword(accountId);

            if(hashedPassword == null)
            {
                return false;
            }

            if(!ValidatePasswordMatchesHash(passwordMatch.Password, hashedPassword))
            {
                return false;
            }

            return true;

        }
    }
}
