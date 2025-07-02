using VaultAPI.DataModel;

namespace VaultAPI.Interfaces.RepoInterfaces
{
    public interface IAccountRepo
    {
        Task<AccountDataModel> AddAsync(AccountDataModel databaseModel);
        Task<AccountDataModel> DeleteAccountViaAccountId(Guid accountId);
        Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel);
        Task<AccountDataModel> UpdateAsync(AccountDataModel databaseModel);
        Task<AccountDataModel> CheckForExistingAccount(Guid accountId);
    }
}