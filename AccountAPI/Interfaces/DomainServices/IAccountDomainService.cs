using KeyForgedShared.DTO_s.AccountDTO_s;
using KeyForgedShared.SharedDataModels;

namespace AccountAPI.Interfaces.DomainServices
{
    public interface IAccountDomainService
    {
        Task<AccountDataModel> DeleteAccount(Guid accountId);
        Task<bool> ValidateCreateAccount(CreateAccountDto createAccount);
        Task<bool> ValidateDeleteAccount(DeleteAccountDto deleteAccount);
    }
}