using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;

namespace ItemProposalAPI.Repository.Interfaces.IEntities
{
    public interface IItemRepository : IRepositoryGeneric<Item, int>
    {
    }
}
