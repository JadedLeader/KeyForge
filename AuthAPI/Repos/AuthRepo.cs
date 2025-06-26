using AccountAPI.DataModel;
using AuthAPI.DataContext;
using AuthAPI.DataModel;
using AuthAPI.Interfaces.RepoInterface;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Formats.Asn1;
using System.Reflection.Metadata.Ecma335;

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

        /// <summary>
        /// This is a slightly more complex method where records must first be checked for within the Auth table, removed, then removed in the account table 
        /// This is due to records persisting within the auth table even if they're removed within the account table
        /// It can be presumed that if there's an existing account ID within the auth table that the same ID still currently exists within the account table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveAccountFromTablesViaId(Guid id)
        {

            AuthDataModel checkingForExistingAuth = await CheckForExistingAuth(id); 

            AccountDataModel checkingForExistingAccount = await CheckForExistingAccount(id);

            if(checkingForExistingAuth.AccountId == Guid.Empty && checkingForExistingAccount.AccountId == Guid.Empty)
            {
                Log.Information($"No accounts can be found in either tables of these records that require deletion");
            }
            else if(checkingForExistingAccount.AccountId != Guid.Empty && checkingForExistingAuth.AccountId == Guid.Empty)
            {
                Log.Information($"Account with ID {id} has been found for deletion within both tables"); 

                _dataContext.Account.Remove(checkingForExistingAccount);

                await _dataContext.SaveChangesAsync() ;
            }

            
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

        public async Task<AuthDataModel> UpdateShortLivedToken(AuthDataModel authDataModel, string refreshedShortLivedToken)
        { 
            authDataModel.ShortLivedKey = refreshedShortLivedToken;

            _dataContext.Update(authDataModel);

            await _dataContext.SaveChangesAsync();
            
            return authDataModel;
        }

        public async Task<AuthDataModel> RevokeLongLivedToken(AuthDataModel authDataModel)
        {
            authDataModel.ShortLivedKey = "";
            authDataModel.LongLivedKey = "";

            _dataContext.Update(authDataModel);

            await _dataContext.SaveChangesAsync();

            return authDataModel;
        }

        public async Task<AuthDataModel> UpdateExistingAuthKeys(AuthDataModel authDataModel, string longLivedKey, string shortLivedKey)
        {
            authDataModel.ShortLivedKey = shortLivedKey; 
            authDataModel.LongLivedKey = longLivedKey;

            _dataContext.Update(authDataModel); 

            await _dataContext.SaveChangesAsync();

            return authDataModel;
        }

        public async Task<AccountDataModel> CheckForExistingAccountViaUsername(string username)
        {
            AccountDataModel? accountDataModel = await _dataContext.Account.Where(ac => ac.Username == username)
                .Select(account =>
                new AccountDataModel
                {
                    AccountId = account.AccountId, 
                    Username = account.Username,
                    Password = account.Password,
                    
                }).FirstOrDefaultAsync();
                

            if(accountDataModel == null)
            {
                return new AccountDataModel(); 
            }

            return accountDataModel;
        }

        public async Task<AccountDataModel> RetrieveRoleFromAccount(Guid accountId)
        {
            AccountDataModel? retrievingRole = await _dataContext.Account.Where(ac => ac.AccountId == accountId)
                .Select(account =>
                new AccountDataModel
                {
                    AuthorisationLevel = account.AuthorisationLevel,
                }).FirstOrDefaultAsync();

            if(retrievingRole == null)
            {
                return new AccountDataModel();
            }

            return retrievingRole;

       
        }

        public async Task<AuthDataModel> CheckForExistingAuthViaAccountId(Guid accountId)
        {
            AuthDataModel? existingAuth = await _dataContext.Auth.Where(ac => ac.AccountId == accountId).FirstOrDefaultAsync();

            if(existingAuth == null)
            {
                return new AuthDataModel();
            }

            return existingAuth;
        }

   
    }
}
