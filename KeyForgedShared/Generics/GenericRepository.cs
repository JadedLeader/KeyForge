using KeyForgedShared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;


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

        public virtual async Task<T?> FindSingleRecordViaId<T>(Guid id) where T : IEntity
        {
            T? model = await _dbContext.Set<T>().Where(t => t.Id == id).FirstOrDefaultAsync();

            if (model == null)
            {
                Log.Information($"{typeof(T)} has failed to find the record");

                return model;
            }

            return model;
        }

        public virtual async Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T: IEntity
        {
            List<T> modelList = await _dbContext.Set<T>().Where(t => t.Id == id).ToListAsync();

            return modelList;
        }

        public virtual async Task<bool> HasModel<T>(Guid id) where T: IEntity
        {
            bool hasRecord = await _dbContext.Set<T>().AnyAsync(x => x.Id == id);

            if (!hasRecord)
            {
                return false;
            }

            return true;
        }



     



    }
}
