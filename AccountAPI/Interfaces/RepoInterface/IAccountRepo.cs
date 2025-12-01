using KeyForgedShared.Interfaces;
using KeyForgedShared.ReturnTypes.Accounts;
using KeyForgedShared.SharedDataModels;

namespace AccountAPI.Interfaces.RepoInterface
{
    public interface IAccountRepo
    {

        public Task CreateAccount(AccountDataModel accountModel);

        public Task<AccountDataModel> DeleteAccount(AccountDataModel accountModel);

        public Task<AccountDataModel> CheckForExistingAccount(Guid accountId);

        public Task<GetAccountDetailsReturn> GetUserAccount(Guid accountId);

        public Task<string> GetHashedPassword(Guid accountId);

        Task<bool> EmailAlreadyExists(string email);

        Task<T?> FindSingleRecordViaId<T>(Guid id) where T : IEntity;

        Task<T> DeleteRecordViaId<T>(Guid id) where T : IEntity;



    }
}
