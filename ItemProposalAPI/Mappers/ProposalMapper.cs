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
                UserId = proposal.User.Id,
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
                Created_At = createProposalDto.Created_At,
                Comment = createProposalDto.Comment,
                Proposal_Status = createProposalDto.Proposal_Status
            };
        }

        public static Proposal ToProposalFromUpdateDto(this UpdateProposalRequestDto updateProposalDto, Proposal proposal)
        {
            proposal.Created_At = updateProposalDto.Created_At;
            proposal.Comment = updateProposalDto.Comment;
            proposal.Proposal_Status = updateProposalDto.Proposal_Status;

            return proposal;
        }
    }
}
