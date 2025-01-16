using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.Models;

namespace ItemProposalAPI.Mappers
{
    public static class PartyMapper
    {
        public static PartyDto ToPartyDto(this Party partyModel)
        {
            return new PartyDto
            {
                Id = partyModel.Id,
                Name = partyModel.Name,
            };
        }

        public static Party ToPartyFromCreateDto(this CreatePartyRequestDto createPartyDto)
        {
            return new Party
            {
                Name = createPartyDto.Name
            };
        }

        public static Party ToPartyFromUpdateDto(this UpdatePartyRequestDto updatePartyDto, Party party)
        {
            party.Name = updatePartyDto.Name;

            return party;
        }
    }
}
