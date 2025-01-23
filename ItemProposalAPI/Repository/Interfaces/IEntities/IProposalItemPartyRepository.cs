using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;
using System.Linq.Expressions;

namespace ItemProposalAPI.Repository.Interfaces.IEntities
{
    public interface IProposalItemPartyRepository 
    {
        Task<IEnumerable<ProposalItemParty>> GetAllAsync(params Expression<Func<ProposalItemParty, object>>[] includes);
        Task<ProposalItemParty?> GetByIdAsync(int proposalId, int itemId, int partyId, params Expression<Func<ProposalItemParty, object>>[] includes);
        Task<ProposalItemParty> AddAsync(ProposalItemParty proposalItemParty);
        /*ProposalItemParty? UpdateAsync(ProposalItemParty proposalItemParty);*/
        Task<ProposalItemParty?> DeleteAsync(int proposalId, int itemId, int partyId);
    }
}
