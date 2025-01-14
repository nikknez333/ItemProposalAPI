namespace ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric
{
    public interface IRepositoryGeneric<TEntity, TPKey> where TEntity : class
    {
        //Generic GET
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TPKey id);
        //Generic POST
        Task<TEntity> CreateAsync(TEntity entity);
        //Generic PUT(UPDATE)
        Task<TEntity?> UpdateAsync(TPKey id, TEntity entity);
        //Generic DELETE
        Task<TEntity?> DeleteAsync(TPKey id);
    }
}
