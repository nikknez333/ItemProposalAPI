using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IEntities;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ItemProposalAPI.Repository.Repositories
{
    public class ProposalItemPartyRepository : IProposalItemPartyRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ProposalItemPartyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProposalItemParty>> GetAllAsync(params Expression<Func<ProposalItemParty, object>>[] includes)
        {
            IQueryable<ProposalItemParty> query = _dbContext.ProposalItemParties;

            foreach(var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<ProposalItemParty?> GetByIdAsync(int proposalId, int itemId, int partyId, params Expression<Func<ProposalItemParty, object>>[] includes)
        {
            IQueryable<ProposalItemParty> query = _dbContext.ProposalItemParties;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(pip => pip.ProposalId == proposalId && pip.ItemId == itemId && pip.PartyId == partyId);
        }

        public async Task<ProposalItemParty> AddAsync(ProposalItemParty proposalItemParty)
        {
            await _dbContext.AddAsync(proposalItemParty);
            return proposalItemParty;
        }

        /*public ProposalItemParty? UpdateAsync(ProposalItemParty proposalItemParty)
        {
            throw new NotImplementedException();
        }*/

        public async Task<ProposalItemParty?> DeleteAsync(int proposalId, int itemId, int partyId)
        {
            var existingModel = await GetByIdAsync(proposalId, itemId, partyId);
            if(existingModel == null)
            {
                return null;
            }

            _dbContext.ProposalItemParties.Remove(existingModel);

            return existingModel;
        }
    }
}
