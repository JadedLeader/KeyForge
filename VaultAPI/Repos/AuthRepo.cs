using AuthAPI.DataModel;
using Microsoft.Identity.Client;
using VaultAPI.DataContext;
using VaultAPI.Repos.GenericRepository;

namespace VaultAPI.Repos
{
    public class AuthRepo : GenericRepository<AuthDataModel>
    {

        private readonly VaultDataContext _dataContext;
        public AuthRepo(VaultDataContext dataContext) : base(dataContext) 
        {
            _dataContext = dataContext; 
        }

        public override Task<AuthDataModel> AddAsync(AuthDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<AuthDataModel> DeleteAsync(AuthDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public override Task<AuthDataModel> UpdateAsync(AuthDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }


    }
}
