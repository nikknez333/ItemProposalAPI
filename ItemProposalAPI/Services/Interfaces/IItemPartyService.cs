using ItemProposalAPI.DTOs.ItemParty;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Validation;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface IItemPartyService
    {
        Task<Result<IEnumerable<ItemPartyDto>>> GetAllAsync(PaginationObject pagination);
        Task<Result<ItemPartyDto>> GetByIdAsync(int partyId, int itemId);
        Task<Result<ItemParty>> AddAsync(CreateItemPartyRequestDto createItemPartyDto);
        Task<Result<ItemParty>> DeleteAsync(int partyId, int itemId);
    }
}
