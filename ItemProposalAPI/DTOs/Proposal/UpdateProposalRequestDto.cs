using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.Proposal
{
    public class UpdateProposalRequestDto
    {
        public DateTime Created_At { get; set; } = DateTime.Now;
        public string? Comment { get; set; } = string.Empty;
        public Proposal_Status Proposal_Status { get; set; }
    }
}
