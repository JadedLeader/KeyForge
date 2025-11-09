using Microsoft.EntityFrameworkCore;


namespace KeyForgedShared.Generics
{
    public class GenericRepository<T> where T : class
    {

        protected readonly DbContext _dbContext;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> AddAsync(T databaseModel)
        { 
            await _dbContext.Set<T>().AddAsync(databaseModel);

            await _dbContext.SaveChangesAsync();

            return databaseModel;
        }

        public virtual async Task<T> DeleteAsync(T databaseModel)
        {
            _dbContext.Set<T>().Remove(databaseModel);

            await _dbContext.SaveChangesAsync(); 

            return databaseModel;
        }

        public virtual async Task<T> UpdateAsync(T databaseModel)
        {
            _dbContext.Set<T>().Update(databaseModel);

            await _dbContext.SaveChangesAsync();

            return databaseModel;
        }

     



    }
}
