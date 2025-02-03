using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;
using System.Linq.Expressions;

namespace ItemProposalAPI.Repository.Interfaces.IEntities
{
    public interface IProposalRepository : IRepositoryGeneric<Proposal, int>
    {
        Task<IEnumerable<Proposal>?> GetNegotiationDetails(int itemId, params Expression<Func<Proposal, object>>[] includes);
    }
}
