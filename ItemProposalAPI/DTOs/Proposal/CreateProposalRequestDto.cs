using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.Proposal
{
    public class CreateProposalRequestDto
    {
        public string? Comment { get; set; } = string.Empty;
    }
}
