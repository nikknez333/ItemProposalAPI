using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.Models;
using System.Security.Claims;

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
                PaymentRatios = proposal.ProposalItemParties.Select(pip => pip.ToPaymentRatioWithStatusDto()).ToList()
            };
        }

       public static ProposalNegotiationDto ToProposalNegotiationDto(this Proposal proposal, User user)
        {
            var proposalCreator = proposal.User.PartyId.Equals(user.PartyId) ? proposal.User.UserName : proposal.User.Party.Name;

            return new ProposalNegotiationDto
            {
                Id = proposal.Id,
                CreatedBy = proposalCreator,
                Created_At = proposal.Created_At,
                Comment = proposal.Comment,
                Proposal_Status = proposal.Proposal_Status,
                CounterToProposalId = proposal.CounterToProposalId,
                PaymentRatios = proposal.ProposalItemParties.Select(pip => pip.ToPaymentRatioNegotationDto(user)).ToList()
            };
        }

        public static Proposal ToProposalFromCreateDto(this CreateProposalRequestDto createProposalDto, string userId)
        {
            return new Proposal
            {
                UserId = userId,
                ItemId = createProposalDto.ItemId,
                Comment = createProposalDto.Comment,
            };
        }

        public static Proposal ToCounterProposalFromCreateDto(this CreateCounterProposalRequestDto createProposalDto, Proposal originalProposal, string userId)
        {
            return new Proposal
            {
                UserId = userId,
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

        public static CreateCounterProposalRequestDto ToCounterProposalFromReviewDto(this ReviewProposalDto evaluateProposalDto)
        {
            return new CreateCounterProposalRequestDto
            {
                Comment = evaluateProposalDto.Comment,
                PaymentRatios = evaluateProposalDto.PaymentRatios
            };
        }
    }
}
