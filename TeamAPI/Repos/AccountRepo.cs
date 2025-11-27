using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TeamAPI.DataContext;
using TeamAPI.Interfaces.Repos;

namespace TeamAPI.Repos
{
    public class AccountRepo : GenericRepository<AccountDataModel>, IAccountRepo
    {
        private readonly TeamApiDataContext _dbContext;
        public AccountRepo(TeamApiDataContext dbContext) :base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public override Task<AccountDataModel> AddAsync(AccountDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public override Task<AccountDataModel> UpdateAsync(AccountDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }

        public async Task<bool> hasAccount(Guid accountId)
        {
            bool accountExists = await _dbContext.Account.AnyAsync(x => x.Id == accountId);

            if (!accountExists)
            {
                return false;
            }

            return true;
        }

    }
}
