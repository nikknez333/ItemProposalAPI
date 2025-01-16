namespace ItemProposalAPI.Models
{
    public class ItemParty
    {
        public int ItemId { get; set; }
        public int PartyId { get; set; }

        //navigation properties
        public Item Item { get; set; }
        public Party Party { get; set; }
        public ICollection<ProposalItemParty> ProposalItemParties { get; set; } = new List<ProposalItemParty>();
    }
}
