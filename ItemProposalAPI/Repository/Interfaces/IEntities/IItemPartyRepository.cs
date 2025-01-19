using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;

namespace ItemProposalAPI.Repository.Interfaces.IEntities
{
    public interface IItemPartyRepository : IRepositoryGeneric<ItemParty, int>
    {
        Task<IEnumerable<Item>> GetPartyItems(int? partyId, QueryObject? query);
        Task<IEnumerable<Party>> GetPartiesSharingItem(int? itemId);
    }
}
