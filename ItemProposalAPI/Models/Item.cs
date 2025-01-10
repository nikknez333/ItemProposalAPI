namespace ItemProposalAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Creation_Date { get; set; } = DateTime.Now;
        public Status Share_Status { get; set; }

        //navigation properties
        public ICollection<Proposal>? Proposals { get; set; }
        public ICollection<ItemParty>? ItemParties { get; set; }
    }

    public enum Status {
        Not_Shared = 0,
        Shared
    }
}
