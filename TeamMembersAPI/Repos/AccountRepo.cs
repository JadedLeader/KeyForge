using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using TeamMembersAPI.DataContext;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.Repos
{
    public class AccountRepo : GenericRepository<AccountDataModel>, IAccountRepo
    {

        private readonly TeamMemberDataContext _accountRepo;

        public AccountRepo(TeamMemberDataContext accountRepo) : base(accountRepo)
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

        public override Task<T> DeleteRecordViaId<T>(Guid id)
        {
            return base.DeleteRecordViaId<T>(id);
        }

        public override Task<List<T>> FindAllRecordsViaId<T>(Guid id)
        {
            return base.FindAllRecordsViaId<T>(id);
        }

        public override Task<T?> FindSingleRecordViaId<T>(Guid id) where T : class
        {
            return base.FindSingleRecordViaId<T>(id);
        }

        public override Task<bool> HasModel<T>(Guid id)
        {
            return base.HasModel<T>(id);
        }

        public override Task<AccountDataModel> UpdateAsync(AccountDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }
    }
}
