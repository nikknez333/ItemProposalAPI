using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ItemProposalAPI.Repository.Repositories
{
    public class PartyRepository : RepositoryGeneric<Party, int>, IPartyRepository
    {
        public PartyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }

        public override async Task<Party?> DeleteAsync(int id)
        {
            var existingParty = await _dbContext.Parties.
                Include(p => p.ItemParties)
                    .ThenInclude(p => p.ProposalItemParties)
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingParty == null)
                return null;

            var proposalIds = existingParty.ItemParties
                .SelectMany(ip => ip.ProposalItemParties)
                .Select(pip => pip.ProposalId)
                .Distinct()
                .ToList();

            var relatedProposals = await _dbContext.Proposals
                .Where(p => proposalIds.Contains(p.Id))
                .ToListAsync();

            _dbContext.Proposals.RemoveRange(relatedProposals);

            _dbContext.Parties.Remove(existingParty);

            return existingParty;
        }
    }
}
