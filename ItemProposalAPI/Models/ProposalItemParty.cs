
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemProposalAPI.Models
{
    public class ProposalItemParty
    {
        public int ProposalId { get; set; }
        public int ItemId { get; set; }
        public int PartyId { get; set; }
        public PaymentType PaymentType { get; set; }
        public decimal PaymentAmount { get; set; }
        public Proposal_Status Response { get; set; }
        public string? UserId { get; set; }

        //navigation properties
        public User? User { get; set; }
        public Proposal Proposal { get; set; }
        public ItemParty ItemParty { get; set; }
    }

    public enum PaymentType
    {
        Percentage = 1,
        Fixed
    } 
}
