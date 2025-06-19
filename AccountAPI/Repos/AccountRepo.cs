using AccountAPI.DataContext;
using AccountAPI.DataModel;
using Microsoft.EntityFrameworkCore.Metadata;
using AccountAPI.Interfaces.RepoInterface;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AccountAPI.Repos
{
    public class AccountRepo : IAccountRepo
    {

        private readonly AccountDataContext _accountDataContext;
        public AccountRepo(AccountDataContext accountDataContext)
        {
            _accountDataContext = accountDataContext;
        }

        public async Task CreateAccount(AccountDataModel accountModel)
        {
            await _accountDataContext.Account.AddAsync(accountModel);

            await _accountDataContext.SaveChangesAsync();
        }

        public async Task<AccountDataModel> DeleteAccount(AccountDataModel accountModel)
        {
            _accountDataContext.Account.Remove(accountModel);

            await _accountDataContext.SaveChangesAsync();

            return accountModel;
        }

        public async Task UpdateAccount()
        {
            throw new NotImplementedException();
        }

        public async Task<AccountDataModel> CheckForExistingAccount(Guid accountId)
        {
           AccountDataModel? retrievedAccount = await _accountDataContext.Account.Where(ac => ac.AccountId == accountId).FirstOrDefaultAsync();

            if(retrievedAccount == null)
            {
                Log.Error($"Could not find an account with the corresponding ID");

                return new AccountDataModel();
            }

            return retrievedAccount;


        }

    }
}
