using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using System.Runtime.CompilerServices;

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

        /*public static ProposalItemParty ToProposalItemPartyFromCreateDto(this CreateProposalItemPartyRequestDto createProposalItemPartyRequestDto)
        {
           
        }*/
    }
}
