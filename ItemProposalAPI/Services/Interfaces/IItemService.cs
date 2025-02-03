using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.Models;
using ItemProposalAPI.Validation;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface IItemService
    {
        Task<Result<IEnumerable<ItemWithoutProposalsDto>>> GetAllAsync();
        Task<Result<ItemWithoutProposalsDto>> GetByIdAsync(int itemId);
        Task<Result<IEnumerable<PartyWithoutUsersDto>>> GetPartiesSharingItemAsync(int itemId);
        Task<Result<ItemNegotiationDto>> GetNegotiationDetails(int itemId);
        Task<Result<Item>> AddAsync(CreateItemRequestDto createItemDto);
        Task<Result<ItemWithoutProposalsDto>> UpdateAsync(int itemId, UpdateItemRequestDto updateItemDto);
        Task<Result<Item>> DeleteAsync(int itemId); 
    }
}
