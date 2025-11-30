using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamInviteAPI.DataContext;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.Repos
{
    public class AccountRepo : GenericRepository<AccountDataModel>, IAccountRepo
    {

        private readonly TeamInviteDataContext _accountRepo;

        public AccountRepo(TeamInviteDataContext accountRepo) : base(accountRepo)
        {
            _accountRepo = accountRepo;
        }

        public override Task<AccountDataModel> AddAsync(AccountDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<AccountDataModel> DeleteAsync(AccountDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public override Task<T> DeleteRecordViaId<T>(Guid id) where T : class
        {
            return base.DeleteRecordViaId<T>(id);
        }

        public override Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T : class
        {
            return base.FindAllRecordsViaId<T>(id);
        }

        public override Task<T?> FindSingleRecordViaId<T>(Guid id) where T : class
        {
            return base.FindSingleRecordViaId<T>(id);
        }

        public override Task<bool> HasModel<T>(Guid id) where T : class
        {
            return base.HasModel<T>(id);
        }

        public override Task<AccountDataModel> UpdateAsync(AccountDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }
    }
}
