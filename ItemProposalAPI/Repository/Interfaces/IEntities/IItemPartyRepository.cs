using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;
using System.Linq.Expressions;

namespace ItemProposalAPI.Repository.Interfaces.IEntities
{
    public interface IItemPartyRepository
    {
        IQueryable<ItemParty> QueryItemParty();
        Task<IEnumerable<Item>> GetPartyItemsAsync(int? partyId, QueryObject? query);
        Task<IEnumerable<Party>> GetPartiesSharingItemAsync(int? itemId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<ItemParty>?> GetAllAsync(int pageNumber, int pageSize, params Expression<Func<ItemParty, object>>[] includes);
        Task<ItemParty?> GetByIdAsync(int partyId, int itemId, params Expression<Func<ItemParty, object>>[] includes);
        Task<ItemParty> AddItemPartyAsync(ItemParty itemParty);
        Task<ItemParty?> RemoveItemPartyAsync(int partyId, int itemId);
    }
}
