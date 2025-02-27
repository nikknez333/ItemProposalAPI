using ItemProposalAPI.DTOs.ProposalItemParty;

namespace ItemProposalAPI.DTOs.Proposal
{
    public class CreateCounterProposalRequestDto
    {
        //public string UserId { get; set; }
        public string Comment { get; set; }
        public List<PaymentRatioDto>? PaymentRatios { get; set; } = new List<PaymentRatioDto>();
    }
}
