using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IEntities;
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

        public async Task<IEnumerable<Proposal>> GetNegotiationDetails(int itemId, params Expression<Func<Proposal, object>>[] includes)
        { 
            IQueryable<Proposal> query = _dbContext.Proposal;

            var proposalsForItem = query
                        .Where(p => p.ItemId == itemId)
                        .OrderBy(p => p.Created_At)
                        .AsQueryable();

            foreach(var include in includes)
            {
                proposalsForItem = proposalsForItem.Include(include);
            }

            return await proposalsForItem.ToListAsync();
        }
    }
}
