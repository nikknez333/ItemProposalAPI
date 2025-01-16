using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public int? PartyId { get; set; }
        [MaxLength(30)]
        public string Username { get; set; } = string.Empty; 
        
        //navigation property
        public Party? Party { get; set; } 
        public ICollection<Proposal>? Proposals { get; set; } = new List<Proposal>();
    }
}
