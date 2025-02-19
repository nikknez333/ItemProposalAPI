using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Validation;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface IPartyService
    {
        Task<Result<IEnumerable<PartyWithoutUsersDto>>> GetAllAsync(PaginationObject pagination);
        Task<Result<PartyDto>> GetByIdAsync(int partyId);
        Task<Result<IEnumerable<ItemWithoutProposalsDto>>> GetPartyItemsAsync(int partyId);
        Task<Result<Party>> AddAsync(CreatePartyRequestDto createPartyDto);
        Task<Result<PartyWithoutUsersDto>> UpdateAsync(int partyId, UpdatePartyRequestDto updatePartyDto);
        Task<Result<Party>> DeleteAsync(int partyId);
    }
}
