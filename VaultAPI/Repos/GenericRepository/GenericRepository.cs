using VaultAPI.DataContext;

namespace VaultAPI.Repos.GenericRepository
{
    public class GenericRepository<T> where T : class
    {

        protected readonly VaultDataContext _vaultDataContext;

        public GenericRepository(VaultDataContext vaultDataContext)
        {
            _vaultDataContext = vaultDataContext;
        }

        public virtual async Task<T> AddAsync(T databaseModel)
        {
            await _vaultDataContext.Set<T>().AddAsync(databaseModel);

            await _vaultDataContext.SaveChangesAsync();

            return databaseModel;
        }

        public virtual async Task<T> DeleteAsync(T databaseModel)
        {
            _vaultDataContext.Set<T>().Remove(databaseModel);

            await _vaultDataContext.SaveChangesAsync(); 

            return databaseModel;
        }

        public virtual async Task<T> UpdateAsync(T databaseModel)
        {
            _vaultDataContext.Set<T>().Update(databaseModel);

            await _vaultDataContext.SaveChangesAsync();

            return databaseModel;
        }



    }
}
