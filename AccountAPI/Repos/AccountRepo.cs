using AccountAPI.DataContext;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore.Metadata;
using AccountAPI.Interfaces.RepoInterface;
using Microsoft.EntityFrameworkCore;
using Serilog;
using KeyForgedShared.ReturnTypes.Accounts;
using KeyForgedShared.Generics;

namespace AccountAPI.Repos
{
    public class AccountRepo : GenericRepository<AccountDataContext>, IAccountRepo
    {

        private readonly AccountDataContext _accountDataContext;
        public AccountRepo(AccountDataContext accountDataContext) : base(accountDataContext)
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

        public async Task<bool> EmailAlreadyExists(string email)
        {
            bool doesEmailAlreadyExist = await _accountDataContext.Account.AnyAsync(x => x.Email == email);

            if (!doesEmailAlreadyExist)
            {
                return false;
            }

            return true;
        }

        public override Task<T?> FindSingleRecordViaId<T>(Guid id) where T : class
        {
            return base.FindSingleRecordViaId<T>(id);
        }

        public override Task<T> DeleteRecordViaId<T>(Guid id) where T: class
        {
            return base.DeleteRecordViaId<T>(id);
        }


    }
}
