using AccountAPI.DataContext;
using AccountAPI.DataModel;
using Microsoft.EntityFrameworkCore.Metadata;
using AccountAPI.Interfaces.RepoInterface;

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

        public async Task DeleteAccount(AccountDataModel accountModel)
        {
            _accountDataContext.Account.Remove(accountModel);

            await _accountDataContext.SaveChangesAsync();
        }

        public async Task UpdateAccount()
        {
            throw new NotImplementedException();
        }

    }
}
