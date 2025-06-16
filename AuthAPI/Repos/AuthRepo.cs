using AccountAPI.DataModel;
using AuthAPI.DataContext;
using AuthAPI.DataModel;
using AuthAPI.Interfaces.RepoInterface;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AuthAPI.Repos
{
    public class AuthRepo : IAuthRepo
    {

        private readonly AuthDataContext _dataContext;
        public AuthRepo(AuthDataContext authDataContext)
        {
            _dataContext = authDataContext; 
        }

        public async Task<AccountDataModel> AddAccountToTable(AccountDataModel accountModel)
        {
            await _dataContext.Account.AddAsync(accountModel);

            await _dataContext.SaveChangesAsync();

            return accountModel;
        }

        public async Task<AccountDataModel> RemoveAccountFromTable(AccountDataModel accountModel)
        {
            AccountDataModel accountViaId = await _dataContext.Account.Where(ac => ac.AccountId == accountModel.AccountId).FirstOrDefaultAsync();

            if(accountViaId == null)
            {
                return new AccountDataModel();
            }

            _dataContext.Remove(accountViaId);

            await _dataContext.SaveChangesAsync();

            return accountViaId;
        }

        public async Task<AccountDataModel> CheckForExistingAccount(Guid accountId)
        {
            AccountDataModel? existingAccount = await _dataContext.Account.Where(ac => ac.AccountId == accountId).FirstOrDefaultAsync();

            if(existingAccount == null)
            {
                Log.Error($"No account exists with this account ID");

                return new AccountDataModel();
            }

            return existingAccount;

        }

        public async Task<AuthDataModel> CheckForExistingAuth(Guid accountId)
        {
            AuthDataModel existingAuth = await _dataContext.Auth.Where(au => au.AccountId == accountId).FirstOrDefaultAsync();   

            if(existingAuth == null)
            {
                Log.Error($"No auth record exists with this auth ID");

                return new AuthDataModel();
            }

            return existingAuth;
        }

        public async Task<AuthDataModel> AddAuthToTable(AuthDataModel authDataModel)
        {
            await _dataContext.Auth.AddAsync(authDataModel);

            await _dataContext.SaveChangesAsync();

            return authDataModel;
            
        }

        public async Task<Guid> RemoveAuthFromTable(AuthDataModel authDataModel)
        {
            AuthDataModel checkingForAuth = await CheckForExistingAuth(authDataModel.AuthKey);

            if(checkingForAuth.AuthKey == Guid.Empty)
            {
                return Guid.Empty;
            }

            _dataContext.Remove(checkingForAuth);

            await _dataContext.SaveChangesAsync();

            return checkingForAuth.AuthKey;
        }

        public async Task<AuthDataModel> UpdateLongLivedToken(AuthDataModel authDataModel, string refreshedLongLivedToken)
        {
            authDataModel.LongLivedKey = refreshedLongLivedToken;

            _dataContext.Update(authDataModel);

            await _dataContext.SaveChangesAsync();

            return authDataModel;
        }

   
    }
}
