using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.Models
{
    public class Proposal
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public int? CounterToProposalId { get; set; } //Null for Initial/Regular Proposal
        public DateTime Created_At { get; set; } = DateTime.Now;
        public string? Comment { get; set; } = string.Empty;
        public Proposal_Status Proposal_Status { get; set; }

        //navigation properties
        public User User { get; set; }
        public Item Item { get; set; }
        public ICollection<ProposalItemParty> ProposalItemParties { get; set; }
        //recursive relationship
        public Proposal InitialProposal { get; set; }
        public ICollection<Proposal>? CounterProposals { get; set; }
    }

    public enum Proposal_Status
    {
        Pending = 0,
        Accepted,
        Rejected
    }
}
