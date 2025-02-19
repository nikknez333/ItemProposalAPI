using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IEntities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace ItemProposalAPI.Repository.Repositories
{
    public class ProposalRepository : RepositoryGeneric<Proposal, int>, IProposalRepository
    {
        public ProposalRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Proposal>?> GetNegotiationDetails(int itemId, int pageNumber, int pageSize)
        { 
            IQueryable<Proposal> query = _dbContext.Proposal;

            var proposalsForItem = query
                        .Where(p => p.ItemId == itemId)
                        .OrderBy(p => p.Created_At)
                        .Include(p => p.User)
                            .ThenInclude(u => u.Party)
                        .Include(p => p.ProposalItemParties)
                            .ThenInclude(pip => pip.User)
                                .ThenInclude(u => u.Party)
                        .Include(p => p.ProposalItemParties)
                            .ThenInclude(pip => pip.ItemParty) 
                                .ThenInclude(ip => ip.Party)
                        .AsQueryable();

            proposalsForItem = proposalsForItem
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await proposalsForItem.ToListAsync();
        }
    }
}
