using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Repository.Interfaces.IEntities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace ItemProposalAPI.Repository.Repositories
{
    public class ItemPartyRepository : IItemPartyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ItemPartyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ItemParty> QueryItemParty()
        {
            return _dbContext.ItemParties.AsQueryable();
        }

        public async Task<IEnumerable<Item>> GetPartyItemsAsync(int? partyid, QueryObject? query)
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

                if (!string.IsNullOrEmpty(query.SortBy.ToString()))
                {
                    if (query.SortBy.ToString().Equals("Name"))
                    {
                        partyItems = query.IsDescending ? partyItems.OrderByDescending(pi => pi.Name) : partyItems.OrderBy(pi => pi.Name);
                    }
                    if(query.SortBy.ToString().Equals("Creation_Date"))
                    {
                        partyItems = query.IsDescending ? partyItems.OrderByDescending(pi => pi.Creation_Date) : partyItems.OrderBy(pi => pi.Creation_Date);
                    }
                    if(query.SortBy.ToString().Equals("Share_Status"))
                    {
                        partyItems = query.IsDescending ? partyItems.OrderByDescending(pi => pi.Share_Status) : partyItems.OrderBy(pi => pi.Share_Status);
                    }
                }

                partyItems = partyItems
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize);
            }

            return await partyItems.ToListAsync();
        }

        public async Task<IEnumerable<Party>> GetPartiesSharingItemAsync(int? itemId, int pageNumber, int pageSize)
        {
            var partiesSharingItem = await _dbContext.ItemParties
                .Where(i => i.ItemId == itemId)
                .Select(p => p.Party)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return partiesSharingItem;
        }

        public async Task<IEnumerable<ItemParty>?> GetAllAsync(int pageNumber, int pageSize, params Expression<Func<ItemParty, object>>[] includes)
        {
            IQueryable<ItemParty> query = _dbContext.ItemParties;

            foreach(var include in includes)
            {
                query = query.Include(include);
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<ItemParty?> GetByIdAsync(int partyId, int itemId, params Expression<Func<ItemParty, object>>[] includes)
        {
            IQueryable<ItemParty> query = _dbContext.ItemParties;
            
            foreach(var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(ip => ip.PartyId == partyId && ip.ItemId == itemId);
        }

        public async Task<ItemParty> AddItemPartyAsync(ItemParty itemParty)
        {
            await _dbContext.ItemParties.AddAsync(itemParty);
            return itemParty;
        }

        public async Task<ItemParty?> RemoveItemPartyAsync(int partyId, int itemId)
        {
            var existingItemParty = await _dbContext.ItemParties
                .Include(ip => ip.ProposalItemParties)
                .FirstOrDefaultAsync(ip => ip.PartyId == partyId && ip.ItemId == itemId);

            if (existingItemParty == null)
                return null;

            var proposalIds = existingItemParty.ProposalItemParties
                .Select(pip => pip.ProposalId)
                .Distinct()
                .ToList();

            var affectedProposals = await _dbContext.Proposals
                .Where(p => proposalIds.Contains(p.Id))
                .ToListAsync();

            _dbContext.Proposals.RemoveRange(affectedProposals);
            _dbContext.Remove(existingItemParty);

            return existingItemParty;
        }

        public async Task<bool> ExistsAsync(int partyId, int itemId)
        {
            return await _dbContext.ItemParties.AnyAsync(ip => ip.PartyId == partyId && ip.ItemId == itemId);
        }
    }
}
