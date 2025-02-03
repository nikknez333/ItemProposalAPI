using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.Proposal
{
    public class ProposalNegotiationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public string CreatedBy { get; set; } 
        public DateTime Created_At { get; set; }
        public string? Comment { get; set; }
        public Proposal_Status Proposal_Status { get; set; }
        public int? CounterToProposalId { get; set; }
        public List<PaymentRatioDto> PaymentRatios { get; set; }
    }
}
