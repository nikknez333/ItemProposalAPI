using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.Models;

namespace ItemProposalAPI.Mappers
{
    public static class ProposalMapper
    {
        public static ProposalDto ToProposalDto(this Proposal proposal)
        {
            return new ProposalDto
            {
                Id = proposal.Id,
                UserId = proposal.UserId,
                ItemId = proposal.ItemId,
                Created_At = proposal.Created_At,
                Comment = proposal.Comment,
                Proposal_Status = proposal.Proposal_Status,
                CounterToProposalId = proposal.CounterToProposalId
            };
        }

        public static Proposal ToProposalFromCreateDto(this CreateProposalRequestDto createProposalDto)
        {
            return new Proposal
            {
                UserId = createProposalDto.UserId,
                ItemId = createProposalDto.ItemId,
                Comment = createProposalDto.Comment,
            };
        }

        public static Proposal ToProposalFromUpdateDto(this UpdateProposalRequestDto updateProposalDto, Proposal proposal)
        {
            proposal.Comment = updateProposalDto.Comment;

            return proposal;
        }
    }
}
