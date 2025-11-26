using KeyForgedShared.Generics;
using KeyForgedShared.Interfaces;
using KeyForgedShared.SharedDataModels;
using TeamVaultAPI.DataContext;
using TeamVaultAPI.Interfaces.Repos;
namespace TeamVaultAPI.Repos
{
    public class AccountRepo : GenericRepository<AccountDataModel>, IAccountRepo
    {

        private readonly TeamVaultDataContext _dbContext;

        public AccountRepo(TeamVaultDataContext dbContext): base(dbContext)
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

        public override Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T : class
        {
            return base.FindAllRecordsViaId<T>(id);
        }

        public override Task<T?> FindSingleRecordViaId<T>(Guid id) where T : class
        {
            return base.FindSingleRecordViaId<T>(id);
        }

        public override Task<bool> HasModel<T>(Guid id) where T: class
        {
            return base.HasModel<T>(id);
        }

    }
}
