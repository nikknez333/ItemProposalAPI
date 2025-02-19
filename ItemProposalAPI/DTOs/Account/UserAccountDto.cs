using ItemProposalAPI.DTOs.Proposal;

namespace ItemProposalAPI.DTOs.Account
{
    public class UserAccountDto
    {
        public string Username { get; set; }
        public int? PartyId { get; set; }
        public string Token { get; set; }
        //public List<ProposalDto> Proposals { get; set; } = new List<ProposalDto>();
    }
}
