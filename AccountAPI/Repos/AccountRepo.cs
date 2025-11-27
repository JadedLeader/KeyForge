using AccountAPI.DataContext;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore.Metadata;
using AccountAPI.Interfaces.RepoInterface;
using Microsoft.EntityFrameworkCore;
using Serilog;
using KeyForgedShared.ReturnTypes.Accounts;

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
           AccountDataModel? retrievedAccount = await _accountDataContext.Account.Where(ac => ac.Id == accountId).FirstOrDefaultAsync();

            if(retrievedAccount == null)
            {
                Log.Error($"Could not find an account with the corresponding ID");

                return new AccountDataModel();
            }

            return retrievedAccount;


        }

        public async Task<GetAccountDetailsReturn> GetUserAccount(Guid accountId)
        {
            GetAccountDetailsReturn? getUser = await _accountDataContext.Account.Where(x => x.Id == accountId)
                .Select(x => new GetAccountDetailsReturn
                {
                    Username = x.Username,
                    Email = x.Email,
                    AccountCreated = x.AccountCreated.ToString(),
                }).FirstOrDefaultAsync();

            if(getUser == null)
            {
                return null; 
            }

            return getUser;
        }

        public async Task<string> GetHashedPassword(Guid accountId)
        {

            string? getPasswordForUser = await _accountDataContext.Account.Where(x => x.Id == accountId)
                .Select(x => x.Password).FirstOrDefaultAsync();

            if(getPasswordForUser == string.Empty)
            {
                return null;
            }

            return getPasswordForUser;

        }

    }
}
