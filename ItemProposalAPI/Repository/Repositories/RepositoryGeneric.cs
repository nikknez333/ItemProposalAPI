using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;
using Microsoft.EntityFrameworkCore;

namespace ItemProposalAPI.Repository.Repositories
{
    public class RepositoryGeneric<TEntity,TPKey> : IRepositoryGeneric<TEntity,TPKey> where TEntity : class
    {
        protected readonly ApplicationDbContext _dbContext;

        public RepositoryGeneric(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TPKey id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public virtual TEntity? UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Attach(entity);
            _dbContext.Set<TEntity>().Entry(entity).State = EntityState.Modified;

            return entity;
        }

        public virtual async Task<TEntity?> DeleteAsync(TPKey id)
        {
            var existingModel = await _dbContext.Set<TEntity>().FindAsync(id);

            if (existingModel == null)
            {
                return null;
            }

            _dbContext.Set<TEntity>().Remove(existingModel);

            return existingModel;
        }
    }
}
