using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.JsonPatch;
using System.Security.Claims;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface IProposalService
    {
        Task<Result<IEnumerable<ProposalDto>>> GetAllAsync(PaginationObject pagination);
        Task<Result<ProposalDto>> GetByIdAsync(int proposalId);
        Task<Result<Proposal>> AddAsync(CreateProposalRequestDto createProposalDto, ClaimsPrincipal User);
        Task<Result<Proposal>> AddCounterProposalAsync(int proposalId, CreateCounterProposalRequestDto createCounterProposalDto, ClaimsPrincipal User);
        Task<Result<ProposalDto>> ReviewProposalAsync(int proposalId, ReviewProposalDto reviewProposalDto, ClaimsPrincipal User);
        /*
        Task<Result<ProposalDto>> AcceptProposalAsync(int proposalId, ClaimsPrincipal User);
        Task<Result<Proposal>> RejectProposalAsync(int proposalId, CreateCounterProposalRequestDto counterProposalDto, ClaimsPrincipal User);*/
        Task<Result<ProposalDto>> UpdateAsync(int proposalId, UpdateProposalRequestDto updateProposalDto);
        Task<Result<Proposal>> DeleteAsync(int proposalId);
    }
}
