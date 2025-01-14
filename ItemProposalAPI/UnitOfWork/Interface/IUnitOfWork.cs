using ItemProposalAPI.Repository.Interfaces.IEntities;

namespace ItemProposalAPI.UnitOfWork.Interface
{
    public interface IUnitOfWork : IDisposable
    { 
        IUserRepository UserRepository { get; }
        IPartyRepository PartyRepository { get; }
        IItemRepository ItemRepository { get; }
        IProposalRepository ProposalRepository { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();
    }
}
