using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.Models
{
    public class Party
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        //navigation properties
        public ICollection<User>? Users { get; set; }

        public ICollection<ItemParty>? ItemParties { get; set; }
    }
}
