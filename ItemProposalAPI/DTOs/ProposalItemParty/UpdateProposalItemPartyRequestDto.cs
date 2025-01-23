using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.ProposalItemParty
{
    public class UpdateProposalItemPartyRequestDto
    {
        public int ProposalId { get; set; }
        public int ItemId { get; set; }
        public int PartyId { get; set; }
        public PaymentType PaymentType { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}
