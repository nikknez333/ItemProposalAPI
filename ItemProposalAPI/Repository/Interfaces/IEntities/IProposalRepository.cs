using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;

namespace ItemProposalAPI.Repository.Interfaces.IEntities
{
    public interface IProposalRepository : IRepositoryGeneric<Proposal, int>
    {
    }
}
