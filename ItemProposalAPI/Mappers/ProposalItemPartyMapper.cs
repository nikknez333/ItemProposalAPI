using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using System.Runtime.CompilerServices;

namespace ItemProposalAPI.Mappers
{
    public static class ProposalItemPartyMapper
    {
        public static ProposalItemPartyDto ToProposalItemPartyDto(this ProposalItemParty proposalItemPartyModel)
        {
            return new ProposalItemPartyDto
            {
                ProposalId = proposalItemPartyModel.ProposalId,
                PartyId = proposalItemPartyModel.PartyId,
                ItemId = proposalItemPartyModel.ItemId,
                PaymentType = proposalItemPartyModel.PaymentType,
                PaymentAmount = proposalItemPartyModel.PaymentAmount,
            };
        }

        /*public static ProposalItemParty ToProposalItemPartyFromCreateDto(this CreateProposalItemPartyRequestDto createProposalItemPartyRequestDto)
        {
           
        }*/
    }
}
