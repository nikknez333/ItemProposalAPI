using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.ProposalItemParty
{
    public class CreateProposalItemPartyRequestDto
    {
        public int ProposalId { get; set; }
        public int ItemId { get; set; }
        public List<PaymentRatioDto> PaymentRatios { get; set; } = new List<PaymentRatioDto>();
    }
}
