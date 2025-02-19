using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace ItemProposalAPI.Repository.Interfaces.IEntities
{
    public interface IProposalRepository : IRepositoryGeneric<Proposal, int>
    {
        Task<IEnumerable<Proposal>?> GetNegotiationDetails(int itemId, int pageNumber = 1, int pageSize = 10);
    }
}
