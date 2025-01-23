using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.Proposal
{
    public class CreateProposalRequestDto
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public string? Comment { get; set; } = string.Empty;
        public List<PaymentRatioDto> PaymentRatios { get; set; } = new List<PaymentRatioDto>();
    }
}
