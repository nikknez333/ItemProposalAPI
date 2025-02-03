using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.Models
{
    public class Party
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        //navigation properties
        public ICollection<User>? Users { get; set; } = new List<User>();

        public ICollection<ItemParty>? ItemParties { get; set; } = new List<ItemParty>();
    }
}
