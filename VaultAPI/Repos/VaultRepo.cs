using VaultAPI.DataContext;
using VaultAPI.DataModel;
using VaultAPI.Interfaces.RepoInterfaces;
using VaultAPI.Repos.GenericRepository;

namespace VaultAPI.Repos
{
    public class VaultRepo : GenericRepository<VaultDataModel>, IVaultRepo
    {

        private readonly VaultDataContext _dataContext;
        public VaultRepo(VaultDataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public override Task<VaultDataModel> AddAsync(VaultDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<VaultDataModel> UpdateAsync(VaultDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }

        public override Task<VaultDataModel> DeleteAsync(VaultDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }



    }
}
