using ItemProposalAPI.DTOs.ItemParty;
using ItemProposalAPI.Models;

namespace ItemProposalAPI.Mappers
{
    public static class ItemPartyMapper
    {
        public static ItemPartyDto ToItemPartyDto(this ItemParty itemPartyModel)
        {
            return new ItemPartyDto
            {
                Party = itemPartyModel.Party.ToPartyWithoutUsersDto(),
                Item = itemPartyModel.Item.ToItemWithoutProposalsDto(),
            };
        }
        public static ItemParty ToItemPartyFromCreateDto(this CreateItemPartyRequestDto createItemPartyDto)
        {
            return new ItemParty
            {
                PartyId = createItemPartyDto.PartyId,
                ItemId = createItemPartyDto.ItemId,
            };
        }

        public static ItemPartyAddResultDto ToItemPartyAddResultDto(this ItemParty itemPartyModel)
        {
            return new ItemPartyAddResultDto
            {
                PartyId = itemPartyModel.PartyId,
                ItemId = itemPartyModel.ItemId,
            };
        }
    }
}
