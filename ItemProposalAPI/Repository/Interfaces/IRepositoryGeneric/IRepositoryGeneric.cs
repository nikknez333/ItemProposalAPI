namespace ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric
{
    public interface IRepositoryGeneric<TEntity, TPKey> where TEntity : class
    {
        //Generic GET
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TPKey id);
        //Generic POST
        Task<TEntity> AddAsync(TEntity entity);
        //Generic PUT(UPDATE)
        TEntity? UpdateAsync(TEntity entity);
        //Generic DELETE
        Task<TEntity?> DeleteAsync(TPKey id);
    }
}
