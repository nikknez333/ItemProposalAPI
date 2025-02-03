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
                CounterToProposalId = proposal.CounterToProposalId,
                PaymentRatios = proposal.ProposalItemParties.Select(pip => pip.ToPaymentRatioDto()).ToList()
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

        public static Proposal ToCounterProposalFromCreateDto(this CreateCounterProposalRequestDto createProposalDto, Proposal originalProposal)
        {
            return new Proposal
            {
                UserId = createProposalDto.UserId,
                ItemId = originalProposal.ItemId,
                Comment = createProposalDto.Comment,
                Proposal_Status = originalProposal.Proposal_Status,
                CounterToProposalId = originalProposal.Id,
            };
        }

        public static Proposal ToProposalFromUpdateDto(this UpdateProposalRequestDto updateProposalDto, Proposal proposal)
        {
            proposal.Comment = updateProposalDto.Comment;

            return proposal;
        }
    }
}
