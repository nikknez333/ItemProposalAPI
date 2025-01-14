using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IEntities;

namespace ItemProposalAPI.Repository.Repositories
{
    public class PartyRepository : RepositoryGeneric<Party, int>, IPartyRepository
    {
        public PartyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
