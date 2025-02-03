using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.Models;
using ItemProposalAPI.Validation;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface IProposalService
    {
        Task<Result<IEnumerable<ProposalDto>>> GetAllAsync();
        Task<Result<ProposalDto>> GetByIdAsync(int proposalId);
        Task<Result<Proposal>> AddAsync(CreateProposalRequestDto createProposalDto);
        Task<Result<Proposal>> AddCounterProposalAsync(int proposalId, CreateCounterProposalRequestDto createCounterProposalDto);
        Task<Result<ProposalDto>> UpdateAsync(int proposalId, UpdateProposalRequestDto updateProposalDto);
        Task<Result<Proposal>> DeleteAsync(int proposalId);
    }
}
