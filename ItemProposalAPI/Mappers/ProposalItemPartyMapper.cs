using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ItemProposalAPI.Mappers
{
    public static class ProposalItemPartyMapper
    {
        public static PaymentRatioDto ToPaymentRatioDto(this ProposalItemParty proposalItemPartyModel)
        {
            return new PaymentRatioDto
            {
                PartyId = proposalItemPartyModel.PartyId,
                PaymentType = proposalItemPartyModel.PaymentType,
                PaymentAmount = proposalItemPartyModel.PaymentAmount,
            };
        }

        public static PaymentRatioWithStatusDto ToPaymentRatioWithStatusDto(this ProposalItemParty proposalItemPartyModel)
        {
            return new PaymentRatioWithStatusDto
            {
                PartyId = proposalItemPartyModel.PartyId,
                PaymentType = proposalItemPartyModel.PaymentType,
                PaymentAmount = proposalItemPartyModel.PaymentAmount,
                Response = proposalItemPartyModel.Response,
                UserId = proposalItemPartyModel.UserId,
            };
        }

        public static PaymentRatioNegotiationDto ToPaymentRatioNegotationDto(this ProposalItemParty proposalItemPartyModel, User user)
        {
            var respondedBy = string.Empty;

            if (proposalItemPartyModel.UserId == null)
                respondedBy = "Not responded";
            else
            {
                respondedBy = proposalItemPartyModel.User.PartyId.Equals(user.PartyId)
                    ? proposalItemPartyModel.User.UserName : proposalItemPartyModel.ItemParty.Party.Name;
            }
            return new PaymentRatioNegotiationDto
            {
                PartyId = proposalItemPartyModel.PartyId,
                PaymentType = proposalItemPartyModel.PaymentType,
                PaymentAmount = proposalItemPartyModel.PaymentAmount,
                Response = proposalItemPartyModel.Response,
                RespondedBy = respondedBy
            };
        }
    }
}
