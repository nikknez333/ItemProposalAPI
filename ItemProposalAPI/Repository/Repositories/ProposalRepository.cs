using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IEntities;

namespace ItemProposalAPI.Repository.Repositories
{
    public class ProposalRepository : RepositoryGeneric<Proposal, int>, IProposalRepository
    {
        public ProposalRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
