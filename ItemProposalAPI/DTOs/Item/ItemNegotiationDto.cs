using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.Item
{
    public class ItemNegotiationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Creation_Date { get; set; } = DateTime.Now;
        public Status Share_Status { get; set; }
        public List<ProposalDto> Proposals { get; set; } = new List<ProposalDto>();
    }
}
