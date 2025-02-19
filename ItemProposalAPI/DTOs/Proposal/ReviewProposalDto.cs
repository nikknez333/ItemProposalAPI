using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.Proposal
{
    public class ReviewProposalDto
    {
        public Proposal_Status? Response { get; set; }
        public string? Comment { get; set; }
        public List<PaymentRatioDto> PaymentRatios { get; set; } = new List<PaymentRatioDto>();
    }
}
