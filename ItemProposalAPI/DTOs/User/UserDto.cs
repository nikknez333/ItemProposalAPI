using ItemProposalAPI.DTOs.Proposal;
using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }
        public int? PartyId { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<ProposalDto> Proposals { get; set; } = new List<ProposalDto>();
    }
}
