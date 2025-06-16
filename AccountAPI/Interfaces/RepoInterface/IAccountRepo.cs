using AccountAPI.DataModel;

namespace AccountAPI.Interfaces.RepoInterface
{
    public interface IAccountRepo
    {

        public Task CreateAccount(AccountDataModel accountModel);

        public Task DeleteAccount(AccountDataModel accountModel);



    }
}
