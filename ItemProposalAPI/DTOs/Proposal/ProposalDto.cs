using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.DTOs.Proposal
{
    public class ProposalDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ItemId { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;
        public string? Comment { get; set; } = string.Empty;
        public Proposal_Status Proposal_Status { get; set; } = Proposal_Status.Pending;
        //Null for Initial/Regular Proposal
        public int? CounterToProposalId { get; set; }
        public List<PaymentRatioWithStatusDto> PaymentRatios { get; set; }
    }
}
