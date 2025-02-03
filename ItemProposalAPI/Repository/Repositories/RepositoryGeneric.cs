using ItemProposalAPI.DataAccess;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ItemProposalAPI.Repository.Repositories
{
    public class RepositoryGeneric<TEntity,TPKey> : IRepositoryGeneric<TEntity,TPKey> where TEntity : class
    {
        protected readonly ApplicationDbContext _dbContext;

        public RepositoryGeneric(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(/*QueryObject queryObject,*/ params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            /*query = query.Skip((queryObject.PageNumber - 1) * queryObject.PageSize)
            .Take(queryObject.PageSize);*/

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TPKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(entity => EF.Property<TPKey>(entity, "Id")!.Equals(id));
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
