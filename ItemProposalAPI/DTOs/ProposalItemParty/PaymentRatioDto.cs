using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.ProposalItemParty
{
    public class PaymentRatioDto
    {
        public int PartyId { get; set; }
        public PaymentType PaymentType { get; set; }  
        public decimal PaymentAmount { get; set; }
    }
}
