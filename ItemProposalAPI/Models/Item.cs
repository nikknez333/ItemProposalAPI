using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Creation_Date { get; set; } = DateTime.Now;
        public Status Share_Status { get; set; } = Status.Not_Shared;

        //navigation properties
        public ICollection<Proposal>? Proposals { get; set; } = new List<Proposal>();
        public ICollection<ItemParty>? ItemParties { get; set; } = new List<ItemParty>();
    }

    public enum Status {
        Not_Shared = 0,
        Shared
    }
}
