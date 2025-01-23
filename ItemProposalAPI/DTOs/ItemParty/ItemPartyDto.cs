using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.ItemParty
{
    public class ItemPartyDto
    {
        public PartyWithoutUsersDto? Party { get; set; }
        public ItemWithoutProposalsDto? Item { get; set; }
    }
}
