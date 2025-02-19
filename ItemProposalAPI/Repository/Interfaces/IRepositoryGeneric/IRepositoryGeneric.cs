using ItemProposalAPI.QueryHelper;
using System.Linq.Expressions;

namespace ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric
{
    public interface IRepositoryGeneric<TEntity, TPKey> where TEntity : class
    {
        IQueryable<TEntity> QueryEntity();
        //Generic GET
        Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> GetByIdAsync(TPKey id, params Expression<Func<TEntity, object>>[] includes);
        //Generic POST
        Task<TEntity> AddAsync(TEntity entity);
        //Generic PUT(UPDATE)
        TEntity? UpdateAsync(TEntity entity);
        //Generic DELETE
        Task<TEntity?> DeleteAsync(TPKey id);
        //Generic EXISTS
        Task<bool> ExistsAsync(TPKey id);
    }
}
