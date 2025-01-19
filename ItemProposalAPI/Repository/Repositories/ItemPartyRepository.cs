using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Repository.Interfaces.IEntities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ItemProposalAPI.Repository.Repositories
{
    public class ItemPartyRepository : RepositoryGeneric<ItemParty, int>, IItemPartyRepository
    {
        public ItemPartyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Item>> GetPartyItems(int? partyid, QueryObject? query)
        {
            var partyItems = _dbContext.ItemParties
                .Where(p => p.PartyId == partyid)
                .Select(i => i.Item)
                .AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrWhiteSpace(query.Name))
                {
                    partyItems = partyItems.Where(pi => pi.Name.Contains(query.Name));
                }
                if (query.From_Creation_Date.HasValue)
                {
                    partyItems = partyItems.Where(pi => pi.Creation_Date >= query.From_Creation_Date);
                }
                if (query.To_Creation_Date.HasValue)
                {
                    partyItems = partyItems.Where(pi => pi.Creation_Date <= query.To_Creation_Date);
                }
                if (query.Share_Status.HasValue)
                {
                    partyItems = partyItems.Where(pi => pi.Share_Status.Equals(query.Share_Status));
                }

                /*if (!string.IsNullOrWhiteSpace(query.SortBy))
                {
                    if (query.SortBy.ToLower().Trim().Equals("name"))
                    {
                        partyItems = query.IsDescending ? partyItems.OrderByDescending(pi => pi.Name) : partyItems.OrderBy(pi => pi.Name);
                    }
                }*/
            }
            return await partyItems.ToListAsync();
        }

        public async Task<IEnumerable<Party>> GetPartiesSharingItem(int? itemId)
        {
            var partiesSharingItem = await _dbContext.ItemParties
                .Where(i => i.ItemId == itemId)
                .Select(p => p.Party)
                .ToListAsync();

            return partiesSharingItem;
        }
    }
}
