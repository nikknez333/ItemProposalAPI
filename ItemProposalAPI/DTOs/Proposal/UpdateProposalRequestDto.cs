using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.Proposal
{
    public class UpdateProposalRequestDto
    {
        public string? Comment { get; set; } = string.Empty;
    }
}
